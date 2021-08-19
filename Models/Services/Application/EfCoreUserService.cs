using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Phrook.Models.Entities;
using Phrook.Models.Exceptions;
using Phrook.Models.InputModels;
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

		public async Task<ListViewModel<BookViewModel>> GetUserBooks(string userId, BookListInputModel input)
		{
			input.Search = input.Search?.Trim();
			//Ricerca
			IQueryable<LibraryBook> baseQuery = dbContext.LibraryBooks;

			switch (input.OrderBy)
			{
				case "Title":
					if (input.Ascending) { baseQuery = baseQuery.OrderBy(libraryBook => libraryBook.Book.Title); }
					else { baseQuery = baseQuery.OrderByDescending(libraryBook => libraryBook.Book.Title); }
					break;
				case "Author":
					if (input.Ascending) { baseQuery = baseQuery.OrderBy(libraryBook => libraryBook.Book.Author); }
					else { baseQuery = baseQuery.OrderByDescending(libraryBook => libraryBook.Book.Author); }
					break;
				case "ReadingState":
					if (input.Ascending) { baseQuery = baseQuery.OrderBy(libraryBook => libraryBook.ReadingState); }
					else { baseQuery = baseQuery.OrderByDescending(libraryBook => libraryBook.ReadingState); }
					break;
				case "Rating":
					if (input.Ascending) { baseQuery = baseQuery.OrderBy(libraryBook => libraryBook.Rating); }
					else { baseQuery = baseQuery.OrderByDescending(libraryBook => libraryBook.Rating); }
					break;
				case "Tag":
					if (input.Ascending) { baseQuery = baseQuery.OrderBy(libraryBook => libraryBook.Tag); }
					else { baseQuery = baseQuery.OrderByDescending(libraryBook => libraryBook.Tag); }
					break;
			}

			// try
			// {
			// 	string _ = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			// }
			// catch (NullReferenceException)
			// {
			// 	throw new UserUnknownException();
			// }

			

			string searchString = input.Search.ToLower();
			IQueryable<BookViewModel> query = baseQuery
			.AsNoTracking()
			//TODO: implementare fuzzy
			.Where(libraryBook => libraryBook.UserId == userId && libraryBook.Book.NormalizedTitle.Contains(searchString))
			.Select(libraryBook =>
			new BookViewModel
			{
				Id = libraryBook.BookId,
				ISBN = libraryBook.Book.Isbn,
				Title = libraryBook.Book.Title,
				Author = libraryBook.Book.Author,
				ImagePath = libraryBook.Book.ImagePath,
				Rating = libraryBook.Rating,
				Tag = libraryBook.Tag,
				ReadingState = libraryBook.ReadingState
			});

			List<BookViewModel> books = await query
			.Skip(input.Offset)
			.Take(input.Limit)
			.ToListAsync();

			int totalCount = await query.CountAsync();
			ListViewModel<BookViewModel> result = new ()
			{
				Results = books,
				TotalCount = totalCount
			};
			return result;
		}

		public async Task<string> GetUserFullName(string userId)
		{
			userId = userId?.Trim();
			if(string.IsNullOrWhiteSpace(userId))
			{
				throw new UserUnknownException();
			}
			string fullName = await dbContext.Users
			.AsNoTracking().Where(user => user.Id == userId)
			.Select(user => user.FullName).SingleOrDefaultAsync();

			return fullName;
		}

		public async Task<ListViewModel<SearchedUserViewModel>> GetUsers(string currentUserId, string fullname)
		{
			fullname = fullname?.Trim();

			string searchString = fullname.ToLower();
			IQueryable<SearchedUserViewModel> query = dbContext.Users
			.AsNoTracking()
			//TODO: implementare fuzzy
			.Where(user => user.Visibility && user.NormalizedFullName.Contains(searchString) && user.Id != currentUserId)
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

		public async Task<bool> IsVisible(string userId)
		{
			userId = userId?.Trim();
			if(string.IsNullOrWhiteSpace(userId))
			{
				throw new UserUnknownException();
			}
			bool isVisible = await dbContext.Users
			.AsNoTracking().Where(user => user.Id == userId)
			.Select(user => user.Visibility).SingleOrDefaultAsync();
			return isVisible;
		}
	}
}