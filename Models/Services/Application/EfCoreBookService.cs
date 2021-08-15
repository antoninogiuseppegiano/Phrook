using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Phrook.Models.Entities;
using Phrook.Models.Exceptions;
using Phrook.Models.InputModels;
using Phrook.Models.Options;
using Phrook.Models.Services.Infrastructure;
using Phrook.Models.ViewModels;
using Z.EntityFramework.Plus;

namespace Phrook.Models.Services.Application
{
	public class EfCoreBookService : IBookService
	{
		private readonly PhrookDbContext dbContext;

		private readonly IOptionsMonitor<BooksOptions> booksOptions;
		private readonly ILogger<EfCoreBookService> logger;
		private readonly IHttpContextAccessor httpContextAccessor;

		public EfCoreBookService(IHttpContextAccessor httpContextAccessor, PhrookDbContext dbContext, IOptionsMonitor<BooksOptions> booksOptions, ILogger<EfCoreBookService> logger)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.logger = logger;
			this.booksOptions = booksOptions;
			this.dbContext = dbContext;
		}

		public async Task<BookDetailViewModel> GetBookByIdAsync(string id)
		{
			logger.LogInformation("Book id: {id} requested.", id);
			BookDetailViewModel book;
			try
			{
				int.TryParse(id, out int intId);
				string userId = "";
				try
				{
					userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				}
				catch (NullReferenceException)
				{
					throw new UserUnknownException();
				}

				var query = dbContext.LibraryBooks
				.AsNoTracking()
				.Where(libraryBook => libraryBook.BookId == intId && libraryBook.UserId == userId)
				.Select(libraryBook =>
				new BookDetailViewModel
				{
					Id = libraryBook.BookId,
					ISBN = libraryBook.Book.Isbn,
					Title = libraryBook.Book.Title,
					Author = libraryBook.Book.Author,
					ImagePath = libraryBook.Book.ImagePath,
					Rating = libraryBook.Rating,
					Tag = libraryBook.Tag,
					ReadingState = libraryBook.ReadingState,
					Description = libraryBook.Book.Description
				});
				book = await query.SingleAsync();
			}
			catch (InvalidOperationException)
			{
				logger.LogWarning("Book {id} not found", id);
				throw new BookNotFoundException(id);
			}

			return book;
		}

		public async Task<ListViewModel<BookViewModel>> GetBooksAsync(BookListInputModel model)
		{
			logger.LogInformation("Book list requested.");

			//Ricerca
			IQueryable<LibraryBook> baseQuery = dbContext.LibraryBooks;

			switch (model.OrderBy)
			{
				case "Title":
					if (model.Ascending) { baseQuery = baseQuery.OrderBy(libraryBook => libraryBook.Book.Title); }
					else { baseQuery = baseQuery.OrderByDescending(libraryBook => libraryBook.Book.Title); }
					break;
				case "Author":
					if (model.Ascending) { baseQuery = baseQuery.OrderBy(libraryBook => libraryBook.Book.Author); }
					else { baseQuery = baseQuery.OrderByDescending(libraryBook => libraryBook.Book.Author); }
					break;
				case "ReadingState":
					if (model.Ascending) { baseQuery = baseQuery.OrderBy(libraryBook => libraryBook.ReadingState); }
					else { baseQuery = baseQuery.OrderByDescending(libraryBook => libraryBook.ReadingState); }
					break;
				case "Rating":
					if (model.Ascending) { baseQuery = baseQuery.OrderBy(libraryBook => libraryBook.Rating); }
					else { baseQuery = baseQuery.OrderByDescending(libraryBook => libraryBook.Rating); }
					break;
				case "Tag":
					if (model.Ascending) { baseQuery = baseQuery.OrderBy(libraryBook => libraryBook.Tag); }
					else { baseQuery = baseQuery.OrderByDescending(libraryBook => libraryBook.Tag); }
					break;
			}

			string userId = "";
			try
			{
				userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}

			IQueryable<BookViewModel> query = baseQuery
			.AsNoTracking()
			//TODO: implementare fuzzy
			.Where(libraryBook => libraryBook.UserId == userId && libraryBook.Book.Title.Contains(model.Search))
			.Select(libraryBook =>
			new BookViewModel
			{
				Id = libraryBook.Book.Id,
				ISBN = libraryBook.Book.Isbn,
				Title = libraryBook.Book.Title,
				Author = libraryBook.Book.Author,
				ImagePath = libraryBook.Book.ImagePath,
				Rating = libraryBook.Rating,
				Tag = libraryBook.Tag,
				ReadingState = libraryBook.ReadingState
			});

			List<BookViewModel> books = await query
			.Skip(model.Offset)
			.Take(model.Limit)
			.ToListAsync();

			int totalCount = await query.CountAsync();
			ListViewModel<BookViewModel> result = new ListViewModel<BookViewModel>
			{
				Results = books,
				TotalCount = totalCount
			};
			return result;
		}

		public async Task<BookDetailViewModel> GetBookByISBNAsync(string ISBN)
		{
			logger.LogInformation("Book id: {ISBN} requested.", ISBN);
			string userId = "";
			try
			{
				userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}
			BookDetailViewModel book;
			try
			{
				book = await dbContext.LibraryBooks
				.AsNoTracking()
				.Where(libraryBook => libraryBook.UserId == userId && libraryBook.Book.Isbn.Equals(ISBN))
				.Select(libraryBook =>
				new BookDetailViewModel
				{
					Id = libraryBook.BookId,
					ISBN = libraryBook.Book.Isbn,
					Title = libraryBook.Book.Title,
					Author = libraryBook.Book.Author,
					ImagePath = libraryBook.Book.ImagePath,
					Rating = libraryBook.Rating,
					Tag = libraryBook.Tag,
					ReadingState = libraryBook.ReadingState,
					Description = libraryBook.Book.Description
				})
				.SingleAsync();
			}
			catch (InvalidOperationException)
			{
				logger.LogWarning("Book {ISBN} not found in local database.", ISBN);
				throw new BookNotFoundException(ISBN);
			}

			return book;
		}

		public async Task<BookDetailViewModel> EditBookAsync(EditBookInputModel inputModel)
		{
			string userId = "";
			try
			{
				userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}
			LibraryBook book = await dbContext.LibraryBooks
				.Where(librarybook => librarybook.UserId == userId && librarybook.BookId == inputModel.Id)
				.FirstOrDefaultAsync();

			book.ChangeRating(inputModel.Rating);
			book.ChangeTag(inputModel.Tag);
			book.ChangeReadingState(inputModel.ReadingState);

			dbContext.Entry(book).Property(book => book.RowVersion).OriginalValue = inputModel.RowVersion;

			try
			{
				await dbContext.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				throw new OptimisticConcurrencyException();
			}
			// catch (DbUpdateException e)
			// {
			// 	throw e; //it doesn't make sense, just for structural reasons
			// }

			return new BookDetailViewModel
			{
				Id = book.BookId,
				ISBN = book.Book.Isbn,
				Title = book.Book.Title,
				Description = book.Book.Description,
				Author = book.Book.Author,
				ImagePath = book.Book.ImagePath,
				Rating = book.Rating,
				Tag = book.Tag,
				ReadingState = book.ReadingState
			};

		}

		public async Task<EditBookInputModel> GetBookForEditingAsync(string id)
		{
			string userId = "";
			try
			{
				userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}
			IQueryable<EditBookInputModel> query;
			try
			{
				int.TryParse(id, out int intId);
				query = dbContext.LibraryBooks
				.AsNoTracking()
				//TODO: implementare fuzzy
				.Where(libraryBook => libraryBook.UserId == userId && libraryBook.BookId == intId)
				.Select(librarybook =>
				new EditBookInputModel
				{
					Id = librarybook.BookId,
					Title = librarybook.Book.Title,

					Rating = librarybook.Rating,
					Tag = librarybook.Tag,
					ReadingState = librarybook.ReadingState,
					RowVersion = librarybook.RowVersion
				});

				EditBookInputModel book = await query.SingleAsync();
				return book;
			}
			catch (InvalidOperationException)
			{
				int.TryParse(id, out int intId);
				throw new BookNotFoundException(intId);
			}
		}

		public async Task RemoveBookFromLibrary(string id)
		{
			string userId = "";
			try
			{
				userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			catch (NullReferenceException)
			{
				throw new UserUnknownException();
			}
			int.TryParse(id, out int intId);
			int affectedRows = await dbContext.LibraryBooks.Where(libraryBook => libraryBook.UserId == userId  && libraryBook.BookId == intId).DeleteAsync();
			if (affectedRows is 1)
			{
				await dbContext.SaveChangesAsync();
			}
			else if (affectedRows is 0)
			{
				throw new BookNotFoundException();
			}
			else
			{
				throw new TooManyRowsException(affectedRows);
			}

			//TODO: Per avere db più snello rimuovere riga del db se nessun utente ha più quel libro?
			// eliminazione dal db con EF Core PLUS
			// int affectedRows = await dbContext.Books.Where(book =>  book.Id == intId).DeleteAsync();
			// if (affectedRows is 1)
			// {
			// 	await dbContext.SaveChangesAsync();
			// }
			// else if (affectedRows is 0)
			// {
			// 	throw new BookNotFoundException();
			// }
			// else
			// {
			// 	throw new TooManyRowsException(affectedRows);
			// }
		}

		
	}
}
