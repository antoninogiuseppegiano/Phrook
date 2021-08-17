using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Phrook.Models.Exceptions;
using Phrook.Models.InputModels;
using Phrook.Models.Services.Application;
using Phrook.Models.Services.HttpClients;
using Phrook.Models.ViewModels;

namespace Phrook.Controllers
{
	public class WishlistController : Controller
	{
		private readonly IWishlistService wishlistService;
		private readonly IGoogleBooksClient gbClient;
		public WishlistController(IWishlistService wishlistService, IGoogleBooksClient gbClient)
		{
			this.gbClient = gbClient;
			this.wishlistService = wishlistService;
		}
		
		public async Task<IActionResult> Index(BookListInputModel input)
		{
			ListViewModel<WishlistViewModel> books = await wishlistService.GetBooksAsync(input);

			ViewData["Title"] = "Lista dei desideri";
			return View(books);
		}

		public async Task<IActionResult> Delete(string bookId)
		{
			try
			{
				await wishlistService.RemoveBookFromWishlist(bookId);
			}
			catch (BookNotFoundException)
			{
				TempData["ErrorMessage"] = "Non è stato possibile rimuovere il libro dalla lista dei desideri.";
				return RedirectToAction(nameof(Index));
			}
			TempData["ConfirmationMessage"] = "Libro rimosso dalla lista dei desideri.";
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> AddToWishlist(string bookId) 
		{
			try
			{
				await wishlistService.AddBookToWishlist(bookId);
			}
			catch
			{
				TempData["ErrorMessage"] = "Non è stato possibile aggiungere il libro alla lista dei desideri.";
				return Redirect(Request.GetTypedHeaders().Referer.ToString());
			}
			TempData["ConfirmationMessage"] = "Libro aggiunto alla lista dei desideri.";
			return RedirectToAction(nameof(Index));
		}
	}	
}