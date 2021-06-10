using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Phrook.Models.Services.Application;
using Phrook.Models.ViewModels;

namespace Phrook.Controllers
{
    public class BooksController : Controller
    {
		private readonly IBookService bookService;
        public BooksController(IBookService bookService)
        {
            this.bookService = bookService;
        }

		// /Books
        public IActionResult Index() 
		{
			List<BookViewModel>  books = bookService.GetBooks();

			//return the view /views/Books/Index
			ViewData["Title"] = "Library";
			return View(books);
		}

		// /Books/Detail/?isbn
		public IActionResult Detail(string isbn) 
		{
			BookDetailViewModel  book = bookService.GetBook(isbn);
			//return the view /views/Books/Index
			string title;
			if(book.Title.Length > 5) {
				title = string.Concat($"{book.Title.Substring(0, 5)}", "...");
			}
			else
			{
				title = book.Title.Substring(0, 5);
			}

			ViewData["Title"] = title;
			return View(book);
		}
    }
}
