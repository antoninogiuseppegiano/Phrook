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

			getCurrentUserId(out string currentUserId);

			//get users list
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
				//user is visible
				string fullName;
				try
				{
					//get the searched user's name
					fullName = await userService.GetUserFullName(userId);
				}
				catch
				{
					throw new UserNotFoundException(userId);
				}

				ViewData["Filter"] = input.Search;

				//get user's book list
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
				//user's not visible
				BookListOfUserViewModel viewModel = new()
				{
					UserId = ""
				};
				ViewData["Title"] = "Utente non trovato";
				return View(viewModel);
			}
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