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
			getCurrentUserId(out string currentUserId);

			//get books list
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
			getCurrentUserId(out string currentUserId);
			
			BookDetailViewModel book;
			book = await _bookService.GetBookByIdAsync(currentUserId, id);

			ViewData["Title"] = Utility._getShortTitle(book.Title);
			return View(book);
		}

		public async Task<IActionResult> Edit([FromRoute] string id)
		{
			
			getCurrentUserId(out string currentUserId);

			//get current book info
			EditBookInputModel inputModel = await _bookService.GetBookForEditingAsync(currentUserId, id);
			ViewData["Title"] = "Modifica libro";
			return View(inputModel);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditBookInputModel inputModel)
		{
			if (ModelState.IsValid)
			{
				getCurrentUserId(out string currentUserId);

				try
				{
					//saving changes
					BookDetailViewModel book = await _bookService.EditBookAsync(currentUserId, inputModel);
					TempData["ConfirmationMessage"] = "Campi aggiornati correttamente.";
					return RedirectToAction(nameof(Detail), new { id = inputModel.BookId });
				}
				catch (OptimisticConcurrencyException)
				{
					//not saved
					ModelState.Clear();
					ModelState.AddModelError("", "Salvataggio interrotto. Le informazioni non sono più aggiornate. Aggiorna la pagina manualmente.");
					return RedirectToAction(nameof(Edit), new { id = inputModel.BookId });
				}
				catch
				{
					//not saved
					ModelState.Clear();
					ModelState.AddModelError("", "Errore nel salvataggio.");
				}
			}

			//saved changes
			ViewData["Title"] = "Modifica libro";
			return View(inputModel);
		}

		public async Task<IActionResult> Search(SearchApiInputModel input)
		{
			getCurrentUserId(out string currentUserId);
			
			ViewData["SearchTitle"] = input.SearchTitle;
			ViewData["SearchAuthor"] = input.SearchAuthor;
			if (string.IsNullOrWhiteSpace(input.SearchTitle + input.SearchAuthor))
			{
				//throw new InvalidApiInputException(input.SearchTitle, input.SearchAuthor);
				return Redirect(Request.GetTypedHeaders().Referer.ToString());
			}

			ListViewModel<SearchedBookViewModel> books;

			try
			{
				//search books list
				books = await _gbClient.GetBooksByTitleAuthorAsync(input.SearchTitle, input.SearchAuthor);
			}
			catch (ApiException)
			{
				books = new() { Results = new() }; ;
			}

			foreach (var b in books.Results)
			{
				//check if current user has the book in library
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

			getCurrentUserId(out string currentUserId);

			BookDetailViewModel book;
			try
			{
				//search book in user's library
				book = await _bookService.GetBookByISBNAsync(currentUserId, searchISBN);
			}
			catch
			{
				//book not found
				book = null;
			}

			if (book is not null)
			{
				//if book found
				ViewData["Title"] = Utility._getShortTitle(book.Title);
				return RedirectToAction(nameof(Detail), new { id = book.Id });
			}
			
			//if book not found
			string bookId = "";
			try
			{
				//search book id (API doesnì't return enough infos from ISBN, we need id)
				bookId = await _gbClient.GetIdFromISBNAsync(searchISBN);
			}
			catch
			{
				throw new BookNotFoundException(searchISBN);
			}
			BookOverviewViewModel overviewViewModel;

			try
			{
				//search book by its id
				overviewViewModel = await _gbClient.GetBookByIdAsync(bookId);
			}
			catch (ApiException)
			{
				//overviewViewModel = null;
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
			getCurrentUserId(out string currentUserId);

			if (string.IsNullOrWhiteSpace(id))
			{
				throw new InvalidApiInputException(id);
			}

			BookDetailViewModel book;
			try
			{
				//search book in user's library
				book = await _bookService.GetBookByIdAsync(currentUserId, id);
			}
			catch (BookNotFoundException) 
			{
				//book not found 
				book = null; 
			}

			if (book is not null)
			{
				//book found in user's library
				ViewData["Title"] = Utility._getShortTitle(book.Title);
				return RedirectToAction(nameof(Detail), new { id = id });
			}

			//book not found in user's library
			BookOverviewViewModel overviewViewModel;
			if (await _bookService.IsBookStoredInBooks(id))
			{//if book is in Books table
				//search book that user doesn't own
				overviewViewModel = await _bookService.GetBookNotAddedInLibaryByIdAsync(id);
				bool isInLibrary = await _bookService.IsBookAddedToLibrary(currentUserId, id);
				overviewViewModel.IsInLibrary = isInLibrary;
				bool isInWishlist = await _bookService.IsBookInWishList(currentUserId, overviewViewModel.Id);
				overviewViewModel.IsInWishlist = isInWishlist;
			}
			else
			{
				//the boook isn't in Books table
				try
				{
					//search book API
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
				//book found
				//does user own book?
				bool isInLibrary = await _bookService.IsBookAddedToLibrary(currentUserId, overviewViewModel.Id);
				overviewViewModel.IsInLibrary = isInLibrary;
				//is book in user's wishlist
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
			getCurrentUserId(out string currentUserId);

			try
			{
				//add book to user's library
				await _bookService.AddBookToLibrary(currentUserId, id);
			}
			catch
			{
				//not added
				TempData["ErrorMessage"] = "Non è stato possibile aggiungere il libro alla libreria.";
				return Redirect(Request.GetTypedHeaders().Referer.ToString());
			}
			//added
			TempData["ConfirmationMessage"] = "Libro aggiunto alla libreria.";
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Delete(string id)
		{
			getCurrentUserId(out string currentUserId);

			try
			{
				//remove book from user's library
				await _bookService.RemoveBookFromLibrary(currentUserId, id);
			}
			catch (BookNotFoundException)
			{
				//not removed
				TempData["ErrorMessage"] = "Non è stato possibile rimuovere il libro dalla libreria.";
				return RedirectToAction(nameof(Index));
			}
			//removed
			TempData["ConfirmationMessage"] = "Libro rimosso dalla libreria.";
			return RedirectToAction(nameof(Index));
		}

		private void getCurrentUserId(out string currentUserId)
		{
			try
			{
				//ClaimType.NameIdentifier is the id of the claim needed (the user id)
				currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}
		}
	}
}
