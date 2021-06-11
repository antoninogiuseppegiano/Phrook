using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index() 
		{
			List<BookViewModel>  books = await bookService.GetBooksAsync();

			//return the view /views/Books/Index
			ViewData["Title"] = "Library";
			return View(books);
		}

		// /Books/Detail/?isbn
		public async Task<IActionResult> Detail(int id) 
		{
			BookDetailViewModel  book = await bookService.GetBookAsync(id);
			//return the view /views/Books/Index
			string title;
			const int maxChar = 12;
			if(book.Title.Length > maxChar) {
				title = string.Concat($"{book.Title.Substring(0, maxChar)}", "...");
			}
			else
			{
				title = book.Title.Substring(0, maxChar);
			}

			ViewData["Title"] = title;
			return View(book);
		}
    }
}
