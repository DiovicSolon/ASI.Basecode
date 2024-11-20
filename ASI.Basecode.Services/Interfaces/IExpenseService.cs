using System;
using System.Collections.Generic;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IExpenseService
    {
        IEnumerable<Expense> GetAllExpenses();
        Expense GetExpenseById(int expenseId);
        void AddExpense(Expense expense);
        void UpdateExpense(Expense expense);
        void DeleteExpense(int expenseId);
    }
}
