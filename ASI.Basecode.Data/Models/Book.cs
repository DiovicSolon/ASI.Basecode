using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Book
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public int Id { get; set; }
        public string Title { get; set; }   
        public string Description { get; set; }
        public string Author { get; set; }

        public DateTime DateTimeCreated { get; set; } = DateTime.Now;
        public Book() { }


    }
}
