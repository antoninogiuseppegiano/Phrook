using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Phrook.Models.Exceptions;
using Phrook.Models.InputModels;
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
		private readonly IHttpContextAccessor httpContextAccessor;
		public BooksController(IHttpContextAccessor httpContextAccessor, IBookService bookService, IGoogleBooksClient gbClient)
		{
			this.httpContextAccessor = httpContextAccessor;
			this._gbClient = gbClient;
			this._bookService = bookService;
		}

		public async Task<IActionResult> Index(BookListInputModel input)
		{
			string currentUserId = "";
			try
			{
				currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}

			ListViewModel<BookViewModel> books = await _bookService.GetBooksAsync(currentUserId, input);

			ViewData["Filter"] = input.Search;

			BookListViewModel viewModel = new()
			{
				Books = books,
				Input = input
			};

			ViewData["Title"] = "Libreria";
			return View(viewModel);
		}

		public async Task<IActionResult> Detail([FromRoute] string id)
		{
			string currentUserId = "";
			try
			{
				currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}
			
			BookDetailViewModel book;
			book = await _bookService.GetBookByIdAsync(currentUserId, id);

			//return the view /views/Books/Index
			ViewData["Title"] = Utility._getShortTitle(book.Title);
			return View(book);
		}

		public async Task<IActionResult> Edit([FromRoute] string id)
		{
			
			string currentUserId = "";
			try
			{
				currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}

			EditBookInputModel inputModel = await _bookService.GetBookForEditingAsync(currentUserId, id);
			//TODO ricarica pagina
			ViewData["Title"] = "Modifica libro";
			return View(inputModel);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditBookInputModel inputModel)
		{
			if (ModelState.IsValid)
			{
				string currentUserId = "";
				try
				{
					currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				}
				catch (NullReferenceException)
				{
					throw new UserUnknownException();
				}

				try
				{
					BookDetailViewModel book = await _bookService.EditBookAsync(currentUserId, inputModel);
					TempData["ConfirmationMessage"] = "Campi aggiornati correttamente.";
					return RedirectToAction(nameof(Detail), new { id = inputModel.BookId });
				}
				catch (OptimisticConcurrencyException)
				{
					ModelState.Clear();
					ModelState.AddModelError("", "Salvataggio interrotto. Le informazioni non sono più aggiornate. Aggiorna la pagina manualmente.");
					return RedirectToAction(nameof(Edit), new { id = inputModel.BookId });
				}
				catch
				{
					ModelState.Clear();
					ModelState.AddModelError("", "Errore nel salvataggio.");
				}
			}

			//TODO ricarica pagina
			ViewData["Title"] = "Modifica libro";
			return View(inputModel);
		}

		public async Task<IActionResult> Search(SearchApiInputModel input)
		{
			string currentUserId = "";
			try
			{
				currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}
			
			ViewData["SearchTitle"] = input.SearchTitle;
			ViewData["SearchAuthor"] = input.SearchAuthor;
			if (string.IsNullOrWhiteSpace(input.SearchTitle + input.SearchAuthor))
			{
				//throw new InvalidApiInputException(input.SearchTitle, input.SearchAuthor);
				return Redirect(Request.GetTypedHeaders().Referer.ToString());
			}

			ListViewModel<SearchedBookViewModel> books;

			//TODO: chiamare servizio che cerca tra i libri posseduti dall'utente in db  ????
			//books = await _bookService.GetBooksByTitleAuthorAsync(input);

			try
			{
				books = await _gbClient.GetBooksByTitleAuthorAsync(input.SearchTitle, input.SearchAuthor);
			}
			catch (ApiException)
			{
				books = new() { Results = new() }; ;
			}

			foreach (var b in books.Results)
			{
				bool isInLibrary = await _bookService.IsBookAddedToLibrary(currentUserId, b.Id);
				b.IsInLibrary = isInLibrary;
			}

			var viewModel = new SearchBookListViewModel
			{
				Books = books,
				Input = input
			};
			ViewData["Title"] = "Ricerca";
			return View(viewModel);
		}

		public async Task<IActionResult> OverviewByISBN(string searchISBN)
		{
			ViewData["SearchISBN"] = searchISBN;
			if (string.IsNullOrWhiteSpace(searchISBN) || !long.TryParse(searchISBN, out _))
			{
				//throw new InvalidApiInputException(searchISBN);
				return Redirect(Request.GetTypedHeaders().Referer.ToString());
			}

			string currentUserId = "";
			try
			{
				currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}

			BookDetailViewModel book;
			try
			{
				book = await _bookService.GetBookByISBNAsync(currentUserId, searchISBN);
			}
			catch
			{
				book = null;
			}

			if (book is not null)
			{
				ViewData["Title"] = Utility._getShortTitle(book.Title);
				return RedirectToAction(nameof(Detail), new { id = book.Id });
			}

			string bookId = await _gbClient.GetIdFromISBNAsync(searchISBN);
			BookOverviewViewModel overviewViewModel;

			try
			{
				overviewViewModel = await _gbClient.GetBookByIdAsync(bookId);
			}
			catch (ApiException)
			{
				overviewViewModel = null;
				throw new BookNotFoundException(searchISBN);
			}

			bool isInLibrary = await _bookService.IsBookAddedToLibrary(currentUserId, overviewViewModel.Id);
			overviewViewModel.IsInLibrary = isInLibrary;
			bool isInWishlist = await _bookService.IsBookInWishList(currentUserId, overviewViewModel.Id);
			overviewViewModel.IsInWishlist = isInWishlist;
			SearchBookViewModel viewModel = new()
			{
				Book = overviewViewModel,
				Search = searchISBN
			};
			ViewData["Title"] = Utility._getShortTitle(overviewViewModel.Title);
			return View("Overview", viewModel);
		}

		public async Task<IActionResult> OverviewById(string id)
		{
			string currentUserId = "";
			try
			{
				currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}

			if (string.IsNullOrWhiteSpace(id))
			{
				throw new InvalidApiInputException(id);
			}

			//se appartiene all'utente
			BookDetailViewModel book;
			try
			{
				book = await _bookService.GetBookByIdAsync(currentUserId, id);
			}
			catch (BookNotFoundException) { book = null; }

			if (book is not null)
			{
				ViewData["Title"] = Utility._getShortTitle(book.Title);
				return RedirectToAction(nameof(Detail), new { id = id });
			}

			BookOverviewViewModel overviewViewModel;
			if (await _bookService.IsBookStoredInBooks(id))
			{
				overviewViewModel = await _bookService.GetBookNotAddedInLibaryByIdAsync(id);
				bool isInLibrary = await _bookService.IsBookAddedToLibrary(currentUserId, id);
				overviewViewModel.IsInLibrary = isInLibrary;
				bool isInWishlist = await _bookService.IsBookInWishList(currentUserId, overviewViewModel.Id);
				overviewViewModel.IsInWishlist = isInWishlist;
			}
			else
			{
				try
				{
					overviewViewModel = await _gbClient.GetBookByIdAsync(id);
					overviewViewModel.IsInLibrary = false;
					overviewViewModel.IsInWishlist = false;
				}
				catch (ApiException)
				{
					throw new BookNotFoundException(id);
				}
			}


			if (overviewViewModel is not null)
			{
				bool isInLibrary = await _bookService.IsBookAddedToLibrary(currentUserId, overviewViewModel.Id);
				overviewViewModel.IsInLibrary = isInLibrary;
				bool isInWishlist = await _bookService.IsBookInWishList(currentUserId, overviewViewModel.Id);
				overviewViewModel.IsInWishlist = isInWishlist;
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
				throw new BookNotFoundException(id);
			}


		}

		public async Task<IActionResult> AddToLibrary(string id)
		{
			string currentUserId = "";
			try
			{
				currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}

			try
			{
				await _bookService.AddBookToLibrary(currentUserId, id);
			}
			catch
			{
				TempData["ErrorMessage"] = "Non è stato possibile aggiungere il libro alla libreria.";
				return Redirect(Request.GetTypedHeaders().Referer.ToString());
			}
			TempData["ConfirmationMessage"] = "Libro aggiunto alla libreria.";
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Delete(string id)
		{
			string currentUserId = "";
			try
			{
				currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}

			try
			{
				await _bookService.RemoveBookFromLibrary(currentUserId, id);
			}
			catch (BookNotFoundException)
			{
				TempData["ErrorMessage"] = "Non è stato possibile rimuovere il libro dalla libreria.";
				return RedirectToAction(nameof(Index));
			}
			TempData["ConfirmationMessage"] = "Libro rimosso dalla libreria.";
			return RedirectToAction(nameof(Index));
		}
	}
}
