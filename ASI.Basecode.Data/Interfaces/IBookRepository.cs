using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBookRepository
    {
        IEnumerable<Book> ViewBooks();
        Book GetBookById(int id);  // Add this method
        void Addbook(Book book);
        void DeleteBook(Book book);
        void EditBook(Book book);
        void AddBook(Book newBook);
    }
}
