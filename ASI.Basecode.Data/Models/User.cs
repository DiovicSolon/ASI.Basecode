using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string ProfilePictureUrl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }

        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
