using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Phrook.Models.Services.Application;
using Phrook.Models.ViewModels;

namespace Phrook.Controllers
{
	public class UsersController : Controller
	{
		private readonly IUserService userService;
		public UsersController(IUserService userService)
		{
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

			ListViewModel<SearchedUserViewModel> users = await userService.GetUsers(searchUser);

			return View(users);
		}
	}
}