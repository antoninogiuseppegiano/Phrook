using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Phrook.Models.Exceptions;
using Phrook.Models.InputModels;
using Phrook.Models.Services.Application;
using Phrook.Models.ViewModels;

namespace Phrook.Controllers
{
	public class WishlistController : Controller
	{
		private readonly IWishlistService wishlistService;
		private readonly IHttpContextAccessor httpContextAccessor;
		public WishlistController(IHttpContextAccessor httpContextAccessor, IWishlistService wishlistService)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.wishlistService = wishlistService;
		}

		public async Task<IActionResult> Index(BookListInputModel input)
		{
			getCurrentUserId(out string currentUserId);

			//get wishlist
			ListViewModel<WishlistViewModel> books = await wishlistService.GetBooksAsync(currentUserId, input);

			ViewData["Title"] = "Lista dei desideri";
			return View(books);
		}

		public async Task<IActionResult> Delete(string bookId)
		{
			getCurrentUserId(out string currentUserId);

			try
			{
				await wishlistService.RemoveBookFromWishlist(currentUserId, bookId);
			}
			catch (BookNotFoundException)
			{
				//not removed
				TempData["ErrorMessage"] = "Non è stato possibile rimuovere il libro dalla lista dei desideri.";
				return RedirectToAction(Request.GetTypedHeaders().Referer.ToString());
			}
			//removed
			TempData["ConfirmationMessage"] = "Libro rimosso dalla lista dei desideri.";
			return RedirectToAction("OverviewById", "Books", new { id = bookId });
		}

		public async Task<IActionResult> AddToWishlist(string bookId)
		{
			getCurrentUserId(out string currentUserId);
			
			try
			{
				await wishlistService.AddBookToWishlist(currentUserId, bookId);
			}
			catch
			{
				//not added
				TempData["ErrorMessage"] = "Non è stato possibile aggiungere il libro alla lista dei desideri.";
				return Redirect(Request.GetTypedHeaders().Referer.ToString());
			}
			//added
			TempData["ConfirmationMessage"] = "Libro aggiunto alla lista dei desideri.";
			return RedirectToAction("OverviewById", "Books", new { id = bookId });
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