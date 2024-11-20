using System;
using System.Collections.Generic;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;

namespace ASI.Basecode.Services.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }
        public IEnumerable<Expense> GetAllExpenses()
        {
            return _expenseRepository.GetAllExpenses();
        }
        public Expense GetExpenseById(int expenseId)
        {
            return _expenseRepository.GetExpenseById(expenseId);
        }

        public void AddExpense(Expense expense)
        {
            _expenseRepository.AddExpense(expense);
        }

        public void UpdateExpense(Expense expense)
        {
            _expenseRepository.UpdateExpense(expense);
        }
        public void DeleteExpense(int expenseId)
        {
            _expenseRepository.DeleteExpense(expenseId);
        }
    }
}
