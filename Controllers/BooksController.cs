using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Phrook.Models.InputModels;
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
        public async Task<IActionResult> Index(BookListInputModel input) 
		{
			ListViewModel<BookViewModel> books = await bookService.GetBooksAsync(input);

			var viewModel = new BookListViewModel
			{
				Books = books,
				Input = input
			};

			//return the view /views/Books/Index
			ViewData["Title"] = "Libreria";
			return View(viewModel);
		}

		// /Books/Detail/?isbn
		public async Task<IActionResult> Detail([FromRoute]int id) 
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
