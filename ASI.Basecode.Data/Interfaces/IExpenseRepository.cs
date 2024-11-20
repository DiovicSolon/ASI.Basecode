using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IExpenseRepository
    {

        IEnumerable<Expense> GetAllExpenses();
        Expense GetExpenseById(int expenseId);
        void AddExpense(Expense expense);
        void UpdateExpense(Expense expense);
        void DeleteExpense(int expenseId);
    }
}
