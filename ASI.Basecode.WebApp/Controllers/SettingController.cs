using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.WebApp.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class SettingController : Controller
    {
        private readonly AsiBasecodeDBContext _context;
        private readonly ILogger<SettingController> _logger;
        private readonly string _uploadsFolder;

        public SettingController(AsiBasecodeDBContext context, ILogger<SettingController> logger)
        {
            _context = context;
            _logger = logger;
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "profiles");

            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        private string GetLoggedInUserName()
        {
            return User.Identity.Name;
        }

        public async Task<IActionResult> ViewSetting()
        {
            try
            {
                string userName = GetLoggedInUserName();
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserName == userName);

                if (user == null)
                {
                    return NotFound();
                }
                UpdateProfilePictureSession(user.ProfilePictureUrl);

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load account details.");
                return View("Error", new ErrorViewModel { ErrorMessage = "Failed to load account details." });
            }
        }




        private void UpdateProfilePictureSession(string profilePictureUrl)
        {
            var pictureUrl = !string.IsNullOrEmpty(profilePictureUrl)
                ? profilePictureUrl
                : "/img/profiles/default-avatar.png";

            HttpContext.Session.SetString("ProfilePicture", pictureUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAccount(User user, IFormFile ProfilePicture)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction(nameof(ViewSetting));
                }

                if (ProfilePicture != null && ProfilePicture.Length > 0)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(ProfilePicture.ContentType.ToLower()))
                    {
                        TempData["ErrorMessage"] = "Invalid file type. Please upload an image file.";
                        return RedirectToAction(nameof(ViewSetting));
                    }

                    if (ProfilePicture.Length > 5 * 1024 * 1024)
                    {
                        TempData["ErrorMessage"] = "File size too large. Maximum size is 5MB.";
                        return RedirectToAction(nameof(ViewSetting));
                    }

                    string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(ProfilePicture.FileName)}";
                    string filePath = Path.Combine(_uploadsFolder, uniqueFileName);

                    if (!string.IsNullOrEmpty(existingUser.ProfilePictureUrl))
                    {
                        string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                            existingUser.ProfilePictureUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePicture.CopyToAsync(fileStream);
                    }

                    existingUser.ProfilePictureUrl = $"/img/profiles/{uniqueFileName}";

                    UpdateProfilePictureSession(existingUser.ProfilePictureUrl);
                }

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.UpdatedBy = GetLoggedInUserName();
                existingUser.UpdatedTime = DateTime.Now;

                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("UserName", $"{existingUser.FirstName} {existingUser.LastName}");

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(ViewSetting));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update account details.");
                TempData["ErrorMessage"] = "Failed to update account details. Please try again.";
                return RedirectToAction(nameof(ViewSetting));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string Password, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ErrorMessage"] = "All password fields are required.";
                return RedirectToAction(nameof(ViewSetting));
            }

            // Validate new password
            if (newPassword.Length < 8)
            {
                TempData["ErrorMessage"] = "New password must be at least 8 characters long.";
                return RedirectToAction(nameof(ViewSetting));
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "New password and confirm password do not match.";
                return RedirectToAction(nameof(ViewSetting));
            }

            try
            {
                string userName = GetLoggedInUserName();
                var user = _context.Users.FirstOrDefault(u => u.UserName == userName);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction(nameof(ViewSetting));
                }

                try
                {
                    string decryptedPassword = PasswordManager.DecryptPassword(user.Password);
                    if (decryptedPassword != Password)
                    {
                        TempData["ErrorMessage"] = "Current password is incorrect.";
                        return RedirectToAction(nameof(ViewSetting));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error decrypting password. Possible data corruption.");
                    TempData["ErrorMessage"] = "Failed to verify the current password. Please contact support.";
                    return RedirectToAction(nameof(ViewSetting));
                }

                user.Password = PasswordManager.EncryptPassword(newPassword);
                user.UpdatedBy = userName;
                user.UpdatedTime = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction(nameof(ViewSetting));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to change password.");
                TempData["ErrorMessage"] = "Failed to change password. Please try again.";
                return RedirectToAction(nameof(ViewSetting));
            }
        }


    }
}