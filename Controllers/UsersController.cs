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
	public class UsersController : Controller
	{
		private readonly IUserService userService;
		private readonly IHttpContextAccessor httpContextAccessor;
		public UsersController(IHttpContextAccessor httpContextAccessor, IUserService userService)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.userService = userService;
		}
		public async Task<IActionResult> Search(string searchUser)
		{
			ViewData["SearchUser"] = searchUser;
			if (string.IsNullOrWhiteSpace(searchUser))
			{
				// throw new ArgumentException();
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

			ListViewModel<SearchedUserViewModel> users = await userService.GetUsers(currentUserId, searchUser);

			SearchUserListViewModel result = new()
			{
				SearchUser = searchUser,
				Users = users
			};

			ViewData["Title"] = $"Ricerca";
			return View(result);
		}

		public async Task<IActionResult> Index(string userId, BookListInputModel input)
		{
			
			if (await userService.IsVisible(userId))
			{
				string fullName;
				try
				{
					fullName = await userService.GetUserFullName(userId);
				}
				catch
				{
					throw new UserNotFoundException(userId);
				}
				ViewData["Filter"] = input.Search;
				ListViewModel<BookViewModel> books = await userService.GetUserBooks(userId, input);

				BookListOfUserViewModel viewModel = new()
				{
					Books = books,
					Input = input,
					FullName = fullName,
					UserId = userId
				};

				ViewData["Title"] = $"Libreria di {fullName}";
				return View(viewModel);
			}
			else
			{
				BookListOfUserViewModel viewModel = new()
				{
					UserId = ""
				};
				ViewData["Title"] = "Utente non trovato";
				return View(viewModel);
			}
		}

	}
}