using System;
using System.Security.Claims;
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
		private readonly IHttpContextAccessor httpContextAccessor;
		public WishlistController(IHttpContextAccessor httpContextAccessor, IWishlistService wishlistService, IGoogleBooksClient gbClient)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.gbClient = gbClient;
			this.wishlistService = wishlistService;
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

			ListViewModel<WishlistViewModel> books = await wishlistService.GetBooksAsync(currentUserId, input);

			ViewData["Title"] = "Lista dei desideri";
			return View(books);
		}

		public async Task<IActionResult> Delete(string bookId)
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
				await wishlistService.RemoveBookFromWishlist(currentUserId, bookId);
			}
			catch (BookNotFoundException)
			{
				TempData["ErrorMessage"] = "Non è stato possibile rimuovere il libro dalla lista dei desideri.";
				return RedirectToAction(Request.GetTypedHeaders().Referer.ToString());
			}
			TempData["ConfirmationMessage"] = "Libro rimosso dalla lista dei desideri.";
			return RedirectToAction("OverviewById", "Books", new { id = bookId });
		}

		public async Task<IActionResult> AddToWishlist(string bookId)
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
				await wishlistService.AddBookToWishlist(currentUserId, bookId);
			}
			catch
			{
				TempData["ErrorMessage"] = "Non è stato possibile aggiungere il libro alla lista dei desideri.";
				return Redirect(Request.GetTypedHeaders().Referer.ToString());
			}
			TempData["ConfirmationMessage"] = "Libro aggiunto alla lista dei desideri.";
			return RedirectToAction("OverviewById", "Books", new { id = bookId });
		}
	}
}