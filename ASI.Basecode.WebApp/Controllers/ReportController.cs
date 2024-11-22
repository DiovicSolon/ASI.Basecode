using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using ASI.Basecode.Services.Interfaces;

namespace ASI.Basecode.WebApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly AsiBasecodeDBContext _context;
        private readonly ICategoryService _categoryService;

        public ReportController(AsiBasecodeDBContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        private string GetLoggedInUserId()
        {
            return HttpContext.User.Identity.Name;
        }

        public IActionResult ViewReport(string category = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var userId = GetLoggedInUserId();

            // Get categories for the filter dropdown
            var categories = _categoryService.GetAllCategory()
                .Where(c => c.UserName == userId)
                .ToList();
            ViewBag.Categories = categories;

            // Set default date range if not provided
            startDate ??= DateTime.Today.AddDays(-30);  // Default to last 30 days
            endDate ??= DateTime.Today;

            // Build the query with filters
            var query = _context.Expenses.Where(e => e.UserName == userId);

            // Apply category filter if selected
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.Category == category);
            }

            // Apply date range filter
            query = query.Where(e => e.Date >= startDate && e.Date <= endDate);

            // Get total expenses for the filtered data
            var totalExpenses = query.Sum(e => e.Amount);

            // Get monthly expenses for the filtered date range
            var monthlyExpenses = query
                .GroupBy(e => e.Date.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Total = g.Sum(e => e.Amount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            // Get months range based on start and end date
            var months = Enumerable.Range(0, ((endDate?.Year * 12 + endDate?.Month) ?? 0) -
                                          ((startDate?.Year * 12 + startDate?.Month) ?? 0) + 1)
                .Select(m => startDate?.AddMonths(m))
                .Where(d => d <= endDate)
                .ToList();

            // Prepare data for all months in range (including zeros for months with no expenses)
            var allMonths = months.Select(date => new
            {
                Label = date?.ToString("MMMM yyyy"),
                Amount = monthlyExpenses.FirstOrDefault(m => m.Month == date?.Month)?.Total ?? 0
            }).ToList();

            // Store filter values for the view
            ViewBag.SelectedCategory = category;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // Prepare chart data
            ViewBag.MonthlyLabels = JsonConvert.SerializeObject(allMonths.Select(m => m.Label));
            ViewBag.MonthlyData = JsonConvert.SerializeObject(allMonths.Select(m => m.Amount));
            ViewBag.TotalExpenses = totalExpenses;

            return View();
        }

        // Add additional report-related actions here
    }
}