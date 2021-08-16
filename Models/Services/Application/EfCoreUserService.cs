using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Phrook.Models.Exceptions;
using Phrook.Models.Services.Infrastructure;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
	public class EfCoreUserService : IUserService
	{
		private readonly PhrookDbContext dbContext;
		private readonly IHttpContextAccessor httpContextAccessor;
		public EfCoreUserService(IHttpContextAccessor httpContextAccessor, PhrookDbContext dbContext)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.dbContext = dbContext;

		}
		public async Task<ListViewModel<SearchedUserViewModel>> GetUsers(string fullname)
		{
			fullname = fullname?.Trim();
			string userId = "";
			try
			{
				userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}

			string searchString = fullname.ToLower();
			IQueryable<SearchedUserViewModel> query = dbContext.Users
			.AsNoTracking()
			//TODO: implementare fuzzy
			.Where(user => user.Visibility && user.NormalizedFullName.Contains(searchString) && user.Id != userId)
			.Select(user =>
			new SearchedUserViewModel
			{
				Id = user.Id,
				FullName = user.FullName
			});

			List<SearchedUserViewModel> users = await query.ToListAsync();

			int totalCount = await query.CountAsync();
			ListViewModel<SearchedUserViewModel> result = new()
			{
				Results = users,
				TotalCount = totalCount
			};
			return result;
		}
	}
}