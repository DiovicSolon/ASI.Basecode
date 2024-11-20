using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASI.Basecode.Data.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; } // Primary key

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } // Name of the category

        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; } // When the category was created

        // Foreign key to User
        [Required(ErrorMessage = "User name is required.")]
        public string UserName { get; set; } // Username of the user who created the category

        // Navigation property (optional, depends on your context)
        [ForeignKey("UserName")]
        public virtual User User { get; set; } // Reference to User model, optional based on your needs
    }
}
