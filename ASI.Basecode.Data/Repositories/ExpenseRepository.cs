using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AsiBasecodeDBContext _context;

        public ExpenseRepository(AsiBasecodeDBContext context)
        {
            _context = context;
        }
        public IEnumerable<Expense> GetAllExpenses()
        {
            return _context.Expenses.ToList();
        }

        public Expense GetExpenseById(int expenseId)
        {
            return _context.Expenses.Find(expenseId);
        }

        public void AddExpense(Expense expense)
        {
            _context.Expenses.Add(expense);
            _context.SaveChanges();
        }


        public void UpdateExpense(Expense expense)
        {
            var existingExpense = _context.Expenses.Find(expense.ExpenseId);
            if (existingExpense != null)
            {
                existingExpense.Title = expense.Title;
                existingExpense.Category = expense.Category;
                existingExpense.Amount = expense.Amount;
                existingExpense.Date = expense.Date;

                _context.SaveChanges();
            }
        }
        public void DeleteExpense(int expenseId)
        {
            var expense = _context.Expenses.Find(expenseId);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                _context.SaveChanges();
            }
        }
    }
}
