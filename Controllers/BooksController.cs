using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Phrook.Models.Exceptions;
using Phrook.Models.InputModels;
using Phrook.Models.ResponseModels;
using Phrook.Models.Services.Application;
using Phrook.Models.Services.HttpClients;
using Phrook.Models.Util;
using Phrook.Models.ViewModels;

namespace Phrook.Controllers
{
	public class BooksController : Controller
	{
		private readonly IGoogleBooksClient _gbClient;
		private readonly IBookService _bookService;
		public BooksController(IBookService bookService, IGoogleBooksClient gbClient)
		{
			this._gbClient = gbClient;
			this._bookService = bookService;
		}

		// /Books
		public async Task<IActionResult> Index(BookListInputModel input)
		{
			ListViewModel<BookViewModel> books = await _bookService.GetBooksAsync(input);

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
		public async Task<IActionResult> Detail([FromRoute] string id)
		{
			BookDetailViewModel book;
			book = await _bookService.GetBookByIdAsync(id);

			//return the view /views/Books/Index
			ViewData["Title"] = Utility._getShortTitle(book.Title);
			return View(book);
		}

		public async Task<IActionResult> Search(SearchApiInputModel input)
		{
			if (string.IsNullOrEmpty(input.SearchTitle + input.SearchAuthor))
			{
				throw new InvalidApiInputException(input.SearchTitle, input.SearchAuthor);
			}

			ListViewModel<SearchedBookViewModel> books;

			//TODO: chiamare servizio che cerca tra i libri posseduti dall'utente in db  ????
			//books = await _bookService.GetBooksByTitleAuthorAsync(input);

			books = await _gbClient.GetBooksByTitleAuthorAsync(input.SearchTitle, input.SearchAuthor);
			//TODO: mettere insieme i risultati delle due chiamate (prendere id unici)

			List<SearchedBookViewModel> paginated = new();
			//TODO: paginare
			for(int i = 0; i < input.Limit; i++) {
				if(i + input.Offset < books.Results.Count) {
					paginated.Add(books.Results[i+input.Offset]);
				}
			}

			var viewModel = new SearchBookListViewModel {
				Books = books,
				Input = input
			};
			ViewData["Title"] = "Ricerca";
			return View(viewModel);
		}
		
		public async Task<IActionResult> OverviewByISBN(string searchISBN)
		{
			if (string.IsNullOrEmpty(searchISBN))
			{
				throw new InvalidApiInputException(searchISBN);
			}

			//TODO: chiamare servizio che cerca tra i libri in db
			try
			{
				BookDetailViewModel book = await _bookService.GetBookByISBNAsync(searchISBN);

				//TODO: se non appartiene all'utente
				bool memberHasBook = false;
				if( memberHasBook)
				{
					ViewData["Title"] = Utility._getShortTitle(book.Title);
					return View("Detail", book);
				}
			} catch (BookNotFoundException e) {}

			string bookId = await _gbClient.GetIdFromISBNAsync(searchISBN);
			BookOverviewViewModel overviewViewModel = await _gbClient.GetBookByIdAsync(bookId);
			var viewModel =  new SearchBookViewModel {
				Book = overviewViewModel,
				Search = searchISBN
			};
			ViewData["Title"] = Utility._getShortTitle(overviewViewModel.Title);
			return View("Overview", viewModel);
			
		}

		public async Task<IActionResult> OverviewById([FromQuery] string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new InvalidApiInputException(id);
			}

			//TODO: chiamare servizio che cerca tra i libri in db
			try
			{
				BookDetailViewModel book = await _bookService.GetBookByIdAsync(id);

				//TODO: se non appartiene all'utente
				bool memberHasBook = false;
				if( memberHasBook)
				{
					ViewData["Title"] = Utility._getShortTitle(book.Title);
					return View("Detail", book);
				}
			}
			catch (BookNotFoundException e){}

			BookOverviewViewModel overviewViewModel = await _gbClient.GetBookByIdAsync(id);
			var viewModel =  new SearchBookViewModel {
				Book = overviewViewModel,
				Search = id
			};
			ViewData["Title"] = Utility._getShortTitle(overviewViewModel.Title);
			return View("Overview", viewModel);
			
		}
	}
}
