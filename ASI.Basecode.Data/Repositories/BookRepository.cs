using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    public class BookRepository:BaseRepository, IBookRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public BookRepository(AsiBasecodeDBContext dbContext, IUnitOfWork unitOfWork): base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Book> ViewBooks()
        {
            return _dbContext.Books.ToList();
        }

        public Book GetBookById(int id)  
        {
            return _dbContext.Books.FirstOrDefault(b => b.Id == id);  
        }

        public void Addbook(Book book)
        {
            _dbContext.Books.Add(book);  
            _dbContext.SaveChanges(); 
        }

        public void DeleteBook(Book book)
        {
            _dbContext.Books.Remove(book);  
            _dbContext.SaveChanges();  
        }

        public void EditBook(Book book)
        {
            var existingBook = _dbContext.Books.FirstOrDefault(b => b.Id == book.Id);
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.Description = book.Description;
                existingBook.Author = book.Author;
                _dbContext.SaveChanges();  
            }
        }

        public void AddBook(Book newBook)
        {
            throw new System.NotImplementedException();
        }
    }
}
