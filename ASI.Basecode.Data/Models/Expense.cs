using System;


namespace ASI.Basecode.Data.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public string UserName { get; set; }

        public User User { get; set; }
    }

}