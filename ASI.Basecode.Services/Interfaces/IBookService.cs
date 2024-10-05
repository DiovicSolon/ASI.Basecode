using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IBookService
    {
        (bool, IEnumerable<Book>) GetBooks();
        Book GetBookById(int id);
        void AddBook(Book book);
        void DeleteBook(Book book);
        void EditBook(Book book);
    }


}
