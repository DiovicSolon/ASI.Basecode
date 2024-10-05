using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }


        public IActionResult Index()
        {
            (bool result, IEnumerable<Book> books) = _bookService.GetBooks();
            if (result)
            {
                return View(books);
            }
            return View();
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View(book);
            }


            _bookService.AddBook(book);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book != null)
            {
                _bookService.DeleteBook(book);
                TempData["Message"] = "The book has been successfully deleted.";
                TempData["AlertType"] = "alert-danger";
            }

            return RedirectToAction(nameof(Index));
        }



        public IActionResult Edit(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View(book);
            }
            TempData["Message"] = "You have successfully Edited";
            TempData["AlertType"] = "alert-success";


            _bookService.EditBook(book);
            return RedirectToAction(nameof(Index));
        }
    }
}
