using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly AsiBasecodeDBContext _context;

        public ReportController(AsiBasecodeDBContext context)
        {
            _context = context;
        }
   
        public IActionResult ViewReport()
        {
            // Fetch total expenses
            var totalExpenses = _context.Expenses.Sum(e => e.Amount);

            // Fetch expenses grouped by month for the chart
            var monthlyExpenses = _context.Expenses
                .GroupBy(e => e.Date.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    Total = group.Sum(e => e.Amount)
                })
                .OrderBy(e => e.Month)
                .ToList();

            // Fetch distinct categories for dropdown
            var categories = _context.Categories
                .Select(c => c.CategoryName)
                .Distinct()
                .ToList();

            // Pass data to the view
            ViewData["TotalExpenses"] = totalExpenses;
            ViewData["MonthlyExpenses"] = monthlyExpenses;
            ViewData["Categories"] = categories;

            return View();
        }

        [HttpGet]
        public IActionResult GenerateReport(string category, DateTime? startDate, DateTime? endDate)
        {
            // Filter expenses based on category and date range
            var filteredExpenses = _context.Expenses.AsQueryable();

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

            var reportData = filteredExpenses.Select(e => new
            {
                e.Title,
                e.Category,
                e.Date,
                e.Amount
            }).ToList();

            // Calculate the total for filtered data
            var totalFilteredExpense = reportData.Sum(e => e.Amount);

            // Return the report view with filtered data
            ViewData["ReportData"] = reportData;
            ViewData["TotalFilteredExpense"] = totalFilteredExpense;

            return View();
        }
    }
}
