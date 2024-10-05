using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public Book GetBookById(int id)
        {
            return _bookRepository.GetBookById(id);
        }

        public (bool, IEnumerable<Book>) GetBooks()
        {
            var books = _bookRepository.ViewBooks();
            return (books != null && books.Any(), books);
        }

        public void AddBook(Book book)
        {
            _bookRepository.Addbook(book);
        }

        public void DeleteBook(Book book)
        {
            _bookRepository.DeleteBook(book);
        }

        public void EditBook(Book book)
        {
            _bookRepository.EditBook(book);
        }
    }
}
