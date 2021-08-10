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

		public async Task<IActionResult> Detail([FromRoute] string id)
		{
			BookDetailViewModel book;
			book = await _bookService.GetBookByIdAsync(id);

			//return the view /views/Books/Index
			ViewData["Title"] = Utility._getShortTitle(book.Title);
			return View(book);
		}

		public async Task<IActionResult> Edit([FromRoute] string id) 
		{
			EditBookInputModel inputModel = await _bookService.GetBookForEditingAsync(id);
			//TODO ricarica pagina
			ViewData["Title"] = "Modifica libro";
			return View(inputModel);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditBookInputModel inputModel) 
		{
			if(!ModelState.IsValid)
			{
				try
				{
					BookDetailViewModel book = await _bookService.EditBookAsync(inputModel);
					TempData["ConfirmationMessage"] = "Campi aggiornati correttamente.";
					return RedirectToAction(nameof(Detail), new { id = inputModel.Id });
				}
				catch (System.Exception)
				{
					
					throw;
				}
			}

			//TODO ricarica pagina
			ViewData["Title"] = "Modifica libro";
			return View(inputModel);
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

			try
			{
				books = await _gbClient.GetBooksByTitleAuthorAsync(input.SearchTitle, input.SearchAuthor);
			}
			catch(ApiException) 
			{
				books = new() {Results = new()};;
			}

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
			if (string.IsNullOrEmpty(searchISBN) || !long.TryParse(searchISBN, out _))
			{
				throw new InvalidApiInputException(searchISBN);
			}

			//TODO: se non appartiene all'utente
			BookDetailViewModel book;
			try
			{
				book = await _bookService.GetBookByISBNAsync(searchISBN);
			} 
			catch
			{
				book = null;
			}

			if(book is not null)
			{
				ViewData["Title"] = Utility._getShortTitle(book.Title);
				return View("Detail", book);
			}
			
			string bookId = await _gbClient.GetIdFromISBNAsync(searchISBN);
			BookOverviewViewModel overviewViewModel;
			
			try {
				overviewViewModel = await _gbClient.GetBookByIdAsync(bookId);
			} 
			catch(ApiException) 
			{
				overviewViewModel = null;
			}

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

			//TODO: se appartiene all'utente
			BookDetailViewModel book;
			try
			{
				book = await _bookService.GetBookByIdAsync(id);
			}
			catch (BookNotFoundException){ book = null; }

			if(book is not null) 
			{
				ViewData["Title"] = Utility._getShortTitle(book.Title);
				return View("Detail", book);
			}
			
			BookOverviewViewModel overviewViewModel;
			try 
			{
				overviewViewModel = await _gbClient.GetBookByIdAsync(id);
			}
			catch(ApiException) 
			{
				throw new BookNotFoundException(int.Parse(id));
			}

			if(overviewViewModel is not null) 
			{
				var viewModel = new SearchBookViewModel
				{
					Book = overviewViewModel,
					Search = id
				};
				ViewData["Title"] = Utility._getShortTitle(overviewViewModel.Title);
				return View("Overview", viewModel);
			}
			else
			{
				throw new BookNotFoundException(int.Parse(id));
			}
			
			
		}
	}
}
