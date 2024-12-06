using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager _signInManager;
        private readonly IUserService _userService;
        private readonly SessionManager _sessionManager;
        private object _context;

        public AccountController(
            SignInManager signInManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            IMapper mapper,
            IUserService userService)
        {
            _signInManager = signInManager;
            _userService = userService;
            _sessionManager = new SessionManager(httpContextAccessor.HttpContext.Session);

        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string email, string newPassword)
        {
            var user = _userService.GetUserByEmail(email);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Email does not exist.";
                return View();
            }
            if (newPassword.Length < 8)
            {
                TempData["ErrorMessage"] = "New password must be at least 8 characters long.";
                return RedirectToAction(nameof(ForgotPassword));
            }

            user.Password = newPassword;
            _userService.UpdateUser(user);

            TempData["SuccessMessage"] = "Password has been updated successfully.";
            return RedirectToAction("Login");

           
        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            TempData["returnUrl"] = System.Net.WebUtility.UrlDecode(HttpContext.Request.Query["ReturnUrl"]);
            _sessionManager.Clear();
            HttpContext.Session.SetString("SessionId", Guid.NewGuid().ToString());
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid login data.";
                return View(model);
            }

            User user = null;
            var loginResult = _userService.AuthenticateUser(model.UserName, model.Password, ref user);
            if (loginResult == LoginResult.Success)
            {
                await _signInManager.SignInAsync(user);
                HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                return RedirectToAction("ExpenseTable", "Expense");
            }

            TempData["ErrorMessage"] = "Incorrect username or password.";
            return View(model);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid registration data.";
                return View(model);
            }
            if (model.Password.Length < 8)
            {
                TempData["ErrorMessage"] = "New password must be at least 8 characters long.";
                return View(model);
            }

            try
            {
                _userService.AddUser(model); // Save the user to the database
                TempData["SuccessMessage"] = "Registration successful. Please log in.";
                return RedirectToAction("Login", "Account"); // Redirect to the login page
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

      }



}
