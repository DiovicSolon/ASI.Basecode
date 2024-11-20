using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        private string GetLoggedInUserId()
        {
            return HttpContext.User.Identity.Name;
        }

        [AllowAnonymous]
        public IActionResult Index(int page = 1, int pageSize = 7)
        {
            try
            {
                string userId = GetLoggedInUserId();

                // Fetch the total count of categories
                var totalCategories = _categoryService.GetAllCategory()
                                                       .Where(c => c.UserName == userId)
                                                       .ToList();

                // Apply a limit to the records per page (limit to 7)
                var categories = totalCategories.Skip((page - 1) * pageSize)
                                                .Take(pageSize)  // Limit to 7 records
                                                .ToList();

                // Calculate total pages
                var totalPages = (int)Math.Ceiling((double)totalCategories.Count / pageSize);

                // Pass categories, current page, and total pages to the view
                ViewData["CurrentPage"] = page;
                ViewData["TotalPages"] = totalPages;
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load categories.");
                return View("Error", new ErrorViewModel { ErrorMessage = "Failed to load categories." });
            }
        }

        public IActionResult Create()
        {
            return View(new Category { DateCreated = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.UserName = GetLoggedInUserId();
                    model.DateCreated = DateTime.Now;
                    _categoryService.AddCategory(model);
                    TempData["SuccessMessage"] = "Category added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add new category.");
                    TempData["ErrorMessage"] = "Failed to add new category. Please try again.";
                }
            }

            return View(model);
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var category = _categoryService.GetCategoryById(id);
                if (category == null || category.UserName != GetLoggedInUserId())
                {
                    TempData["ErrorMessage"] = "Category not found or access denied.";
                    return RedirectToAction(nameof(Index));
                }

                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category for edit.");
                TempData["ErrorMessage"] = "Error retrieving category. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = _categoryService.GetCategoryById(model.CategoryId);
                    if (existingCategory == null || existingCategory.UserName != GetLoggedInUserId())
                    {
                        TempData["ErrorMessage"] = "Category not found or access denied.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Preserve the original DateCreated and UserName
                    model.DateCreated = existingCategory.DateCreated;
                    model.UserName = existingCategory.UserName;

                    _categoryService.UpdateCategory(model);
                    TempData["SuccessMessage"] = "Category updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update category.");
                    TempData["ErrorMessage"] = "Failed to update category. Please try again.";
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var category = _categoryService.GetCategoryById(id);
                if (category == null || category.UserName != GetLoggedInUserId())
                {
                    TempData["ErrorMessage"] = "Category not found or access denied.";
                    return RedirectToAction(nameof(Index));
                }

                _categoryService.DeleteCategory(id);
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete category.");
                TempData["ErrorMessage"] = "Failed to delete category. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            try
            {
                var category = _categoryService.GetCategoryById(id);
                if (category == null || category.UserName != GetLoggedInUserId())
                {
                    TempData["ErrorMessage"] = "Category not found or access denied.";
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category details.");
                TempData["ErrorMessage"] = "Error retrieving category details. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}