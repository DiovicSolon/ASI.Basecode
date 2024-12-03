using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Models;

using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly ICategoryService _categoryService; // Add this line to inject category service
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(IExpenseService expenseService, ICategoryService categoryService, ILogger<ExpenseController> logger)
        {
            _expenseService = expenseService;
            _categoryService = categoryService; // Initialize the category service
            _logger = logger;
        }

        private string GetLoggedInUserId()
        {
            return HttpContext.User.Identity.Name;
        }

        public IActionResult Index()
        {
            try
            {
                string userId = GetLoggedInUserId();
                var expenses = _expenseService.GetAllExpenses()
                                              .Where(e => e.UserName == userId)
                                              .ToList();

                return View(expenses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load expenses.");
                return View("Error", new ErrorViewModel { ErrorMessage = "Failed to load expenses." });
            }
        }
        public IActionResult ExpenseTable(int page = 1, int pageSize = 7, string category = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                string userId = GetLoggedInUserId();

                // Fetch categories for the user
                var categories = _categoryService.GetAllCategory()
                                                 .Where(c => c.UserName == userId)
                                                 .ToList();
                ViewBag.Categories = categories;

                // Filter expenses based on user, category, and date range
                var filteredExpenses = _expenseService.GetAllExpenses()
                                                      .Where(e => e.UserName == userId);

                if (!string.IsNullOrEmpty(category))
                {
                    filteredExpenses = filteredExpenses.Where(e => e.Category == category);
                }

                if (startDate.HasValue)
                {
                    filteredExpenses = filteredExpenses.Where(e => e.Date >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    filteredExpenses = filteredExpenses.Where(e => e.Date <= endDate.Value);
                }

                // Sort expenses by Date (latest on top)
                filteredExpenses = filteredExpenses.OrderByDescending(e => e.Date);

                // Pagination logic
                var totalExpenses = filteredExpenses.Count();
                var totalPages = (int)Math.Ceiling((double)totalExpenses / pageSize);

                page = Math.Max(1, Math.Min(page, totalPages));
                var paginatedExpenses = filteredExpenses.Skip((page - 1) * pageSize)
                                                        .Take(pageSize)
                                                        .ToList();

                // Pass data to ViewData
                ViewData["CurrentPage"] = page;
                ViewData["TotalPages"] = totalPages;
                ViewData["SelectedCategory"] = category;
                ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
                ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

                return View(paginatedExpenses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load expense table.");
                return View("Error", new ErrorViewModel { ErrorMessage = "Failed to load expense table." });
            }
        }


        public IActionResult Create()
        {
            var userName = GetLoggedInUserId(); // Get the logged-in user's username
            var categories = _categoryService.GetAllCategory()
                .Where(c => c.UserName == userName) // Filter categories by UserName
                .ToList();

            ViewBag.Categories = categories; // Pass categories to the view
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Expense model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.UserName = GetLoggedInUserId();
                    _expenseService.AddExpense(model);
                    TempData["SuccessMessage"] = "Expense added successfully!";
                    return RedirectToAction(nameof(ExpenseTable));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add new expense.");
                    TempData["ErrorMessage"] = "Failed to add new expense. Please try again.";
                }
            }

            // Re-populate categories on validation error
            var userName = GetLoggedInUserId();
            var categories = _categoryService.GetAllCategory()
                .Where(c => c.UserName == userName)
                .ToList();

            ViewBag.Categories = categories;
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var expense = _expenseService.GetExpenseById(id);
            if (!HasAccessToExpense(expense))
            {
                TempData["ErrorMessage"] = "Expense not found or access denied.";
                return RedirectToAction(nameof(ExpenseTable));
            }

            // Pass categories to the view for editing
            var userName = GetLoggedInUserId();
            var categories = _categoryService.GetAllCategory()
                .Where(c => c.UserName == userName)
                .ToList();

            ViewBag.Categories = categories;

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Expense model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!HasAccessToExpense(model))
                    {
                        TempData["ErrorMessage"] = "Access denied.";
                        return RedirectToAction(nameof(ExpenseTable));
                    }

                    _expenseService.UpdateExpense(model);
                    TempData["SuccessMessage"] = "Expense updated successfully!";
                    return RedirectToAction(nameof(ExpenseTable));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update expense.");
                    TempData["ErrorMessage"] = "Failed to update expense. Please try again.";
                }
            }

            // Re-populate categories on validation error
            var userName = GetLoggedInUserId();
            var categories = _categoryService.GetAllCategory()
                .Where(c => c.UserName == userName)
                .ToList();

            ViewBag.Categories = categories;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var expense = _expenseService.GetExpenseById(id);
                if (expense == null || expense.UserName != GetLoggedInUserId())
                {
                    TempData["ErrorMessage"] = "Expense not found or access denied.";
                    return RedirectToAction(nameof(ExpenseTable));
                }

                _expenseService.DeleteExpense(id);
                TempData["SuccessMessage"] = "Expense deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete expense.");
                TempData["ErrorMessage"] = "Failed to delete expense. Please try again.";
            }
            return RedirectToAction(nameof(ExpenseTable));
        }

        public IActionResult Details(int id)
        {
            var expense = _expenseService.GetExpenseById(id);
            if (!HasAccessToExpense(expense))
            {
                TempData["ErrorMessage"] = "Expense not found or access denied.";
                return RedirectToAction(nameof(ExpenseTable));
            }
            return View(expense);
        }

        private bool HasAccessToExpense(Expense expense)
        {
            return expense != null && expense.UserName == GetLoggedInUserId();
        }
    }
}
