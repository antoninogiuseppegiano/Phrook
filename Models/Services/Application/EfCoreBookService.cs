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
using Phrook.Models.Services.HttpClients;
using Phrook.Models.Services.Infrastructure;
using Phrook.Models.Util;
using Phrook.Models.ViewModels;
using Z.EntityFramework.Plus;

namespace Phrook.Models.Services.Application
{
	public class EfCoreBookService : IBookService
	{
		private readonly PhrookDbContext dbContext;

		private readonly IOptionsMonitor<BooksOptions> booksOptions;
		private readonly ILogger<EfCoreBookService> logger;
		private readonly IGoogleBooksClient _gbClient;

		public EfCoreBookService(
			IGoogleBooksClient googleBooksClient,
			PhrookDbContext dbContext,
			IOptionsMonitor<BooksOptions> booksOptions,
			ILogger<EfCoreBookService> logger)
		{
			this._gbClient = googleBooksClient;
			this.logger = logger;
			this.booksOptions = booksOptions;
			this.dbContext = dbContext;
		}

		public async Task<BookDetailViewModel> GetBookByIdAsync(string currentUserId, string id)
		{
			logger.LogInformation("Book id: {id} requested.", id);
			BookDetailViewModel book;
			try
			{
				var query = dbContext.LibraryBooks
				.AsNoTracking()
				.Where(libraryBook => libraryBook.BookId == id && libraryBook.UserId == currentUserId)
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
					Description = libraryBook.Book.Description,
					InitialTime = libraryBook.InitialTime,
					FinalTime = libraryBook.FinalTime
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

		public async Task<ListViewModel<BookViewModel>> GetBooksAsync(string currentUserId, BookListInputModel model)
		{
			logger.LogInformation("Book list requested.");
			model.Search = model.Search?.Trim();
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

			string searchString = model.Search.ToLower();
			IQueryable<BookViewModel> query = baseQuery
			.AsNoTracking()
			//TODO: implementare fuzzy
			.Where(libraryBook => libraryBook.UserId == currentUserId && libraryBook.Book.NormalizedTitle.Contains(searchString))
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

			int totalCount = await query.CountAsync();
			if(totalCount == model.Offset)
			{
				model.Offset -= model.Limit;
				model.Page--;
			}
			else if(totalCount < model.Offset)
			{
				var newOffset = (totalCount - totalCount%model.Limit);
				if(newOffset == model.Offset)
				{
					newOffset -= model.Limit;
				}
				model.Offset =  newOffset;
				model.Page = model.Offset/model.Limit;
			}
			// else
			// {
			// 	model.Offset = (totalCount - totalCount%model.Limit) - model.Limit;
			// }

			List<BookViewModel> books = await query
			.Skip(model.Offset)
			.Take(model.Limit)
			.ToListAsync();

			ListViewModel<BookViewModel> result = new ()
			{
				Results = books,
				TotalCount = totalCount
			};
			return result;
		}

		public async Task<BookDetailViewModel> GetBookByISBNAsync(string currentUserId, string ISBN)
		{
			logger.LogInformation("Book ISBN: {ISBN} requested.", ISBN);
			
			BookDetailViewModel book;
			try
			{
				book = await dbContext.LibraryBooks
				.AsNoTracking()
				.Where(libraryBook => libraryBook.UserId == currentUserId && libraryBook.Book.Isbn.Equals(ISBN))
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
					Description = libraryBook.Book.Description,
					InitialTime = libraryBook.InitialTime,
					FinalTime = libraryBook.FinalTime
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

		public async Task<BookDetailViewModel> EditBookAsync(string currentUserId, EditBookInputModel inputModel)
		{
			LibraryBook book = await dbContext.LibraryBooks
				.Where(librarybook => librarybook.UserId == currentUserId && librarybook.BookId == inputModel.BookId)
				.Include(libraryBook => libraryBook.Book)
				.FirstOrDefaultAsync();

			book.ChangeRating(inputModel.Rating);
			if(!string.IsNullOrEmpty(inputModel.Tag))
			{
				book.ChangeTag(inputModel.Tag);
			}
			if(!string.IsNullOrEmpty(inputModel.ReadingState))
			{
				book.ChangeReadingState(inputModel.ReadingState);
			}

			switch(inputModel.ReadingState)
			{
				case "0":
					book.ChangeInitialTime(DateTime.MinValue.Date);
					book.ChangeFinalTime(DateTime.MinValue.Date);
					break;
				case "1":
						book.ChangeInitialTime(inputModel.InitialTime);
					book.ChangeFinalTime(DateTime.MinValue.Date);
					break;
				case "2":
				case "3":
						book.ChangeInitialTime(inputModel.InitialTime);
						book.ChangeFinalTime(inputModel.FinalTime);
					break;
				default:
					book.ChangeInitialTime(DateTime.MinValue.Date);
					book.ChangeFinalTime(DateTime.MinValue.Date);
					break;
				
			}

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
				ReadingState = book.ReadingState,
				InitialTime = book.InitialTime,
				FinalTime = book.FinalTime
			};
		}

		public async Task<EditBookInputModel> GetBookForEditingAsync(string currentUserId, string id)
		{
			IQueryable<EditBookInputModel> query;
			try
			{
				query = dbContext.LibraryBooks
				.AsNoTracking()
				.Where(libraryBook => libraryBook.UserId == currentUserId && libraryBook.BookId == id)
				.Select(libraryBook =>
				new EditBookInputModel
				{
					BookId = libraryBook.BookId,
					Title = libraryBook.Book.Title,

					Rating = libraryBook.Rating,
					Tag = libraryBook.Tag,
					ReadingState = libraryBook.ReadingState,
					InitialTime = Utility._getDateTimeFromyyyy_MM_dd(libraryBook.InitialTime),
					FinalTime = Utility._getDateTimeFromyyyy_MM_dd(libraryBook.FinalTime),
					RowVersion = libraryBook.RowVersion
				});

				EditBookInputModel book = await query.SingleAsync();
				return book;
			}
			catch (InvalidOperationException)
			{
				throw new BookNotFoundException(id);
			}
		}

		public async Task RemoveBookFromLibrary(string currentUserId, string bookId)
		{
			//EF Core Plus cannot work with ObjectContext as DbContext (mocked in testing)
			//int affectedRows = await dbContext.LibraryBooks.Where(libraryBook => libraryBook.UserId == currentUserId && libraryBook.BookId == bookId).DeleteAsync();
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
			
			LibraryBook libraryBook = await dbContext.LibraryBooks.Where(libraryBook => libraryBook.UserId == currentUserId && libraryBook.BookId == bookId).SingleOrDefaultAsync();
            if (libraryBook == null)
            {
                throw new BookNotFoundException(bookId);
            }
			dbContext.Remove(libraryBook);
            await dbContext.SaveChangesAsync();

			//Per avere db più snello, rimuove riga del db se nessun utente ha più quel libro
			bool isStillInLibrary = await dbContext.LibraryBooks.Where(libraryBook => libraryBook.BookId == bookId).CountAsync() > 0;
			if(!isStillInLibrary)
			{
				//EF Core Plus cannot work with ObjectContext as DbContext (mocked in testing)
				// affectedRows = await dbContext.Books.Where(book =>  book.BookId == bookId).DeleteAsync();
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

				Book book = await dbContext.Books.Where(book => book.BookId == bookId).SingleOrDefaultAsync();
				if (book == null)
				{
					throw new BookNotFoundException(bookId);
				}
				dbContext.Remove(book);
				await dbContext.SaveChangesAsync();
			}
			
		}

		public async Task AddBookToLibrary(string currentUserId, string bookId)
		{
			LibraryBook libraryBook;
			if (!(await IsBookStoredInBooks(bookId)))
			{
				BookOverviewViewModel overview = new(); 
				try 
				{
					overview = await _gbClient.GetBookByIdAsync(bookId);
				}
				catch
				{
					throw new BookNotAddedException(bookId);
				}
				if(string.IsNullOrWhiteSpace(overview.Description))
				{
					overview.Description = "Nessuna descrizione";
				}
				Book book = new(overview.Id, overview.ISBN, overview.Title, overview.Author, overview.ImagePath, overview.Description);
				dbContext.Add(book);
				// try
				// {
				// 	await dbContext.SaveChangesAsync();
				// }
				// catch (DbUpdateException)
				// {
				// 	throw new BookNotAddedException(bookId);
				// }
			}

			libraryBook = new(bookId, currentUserId);

			dbContext.Add(libraryBook);
			try
			{
				await dbContext.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				throw new BookNotAddedException(bookId);
			}

			//Removing from wishlist
			//EF Core Plus cannot work with ObjectContext as DbContext (mocked in testing)
			// int affectedRows = await dbContext.Wishlist.Where(wishlist => wishlist.BookId == bookId && wishlist.UserId == currentUserId).DeleteAsync();
			// if (affectedRows is 1)
			// {
			// 	await dbContext.SaveChangesAsync();
			// }
			// else if( affectedRows > 1)
			// {
			// 	throw new TooManyRowsException(affectedRows);
			// }
			Wishlist wishlist = await dbContext.Wishlist.Where(libraryBook => libraryBook.UserId == currentUserId && libraryBook.BookId == bookId).SingleOrDefaultAsync();
            if (wishlist != null)
            {
               	dbContext.Remove(wishlist);
            	await dbContext.SaveChangesAsync();
            }
			
		}

		public async Task<bool> IsBookStoredInBooks(string bookId)
		{
			bool isStored = await dbContext.Books.AnyAsync(book => EF.Functions.Like(book.BookId, bookId));
			return isStored;
		}

		public async Task<bool> IsBookInWishList(string currentUserId, string bookId)
		{
			bool isStored = await dbContext.Wishlist.AnyAsync(book => EF.Functions.Like(book.BookId, bookId) && EF.Functions.Like(book.UserId, currentUserId));
			return isStored;
		}

		public async Task<BookOverviewViewModel> GetBookNotAddedInLibaryByIdAsync(string id)
		{
			BookOverviewViewModel book;
			try
			{
				var query = dbContext.Books
				.AsNoTracking()
				.Where(book => book.BookId == id)
				.Select(book =>
				new BookOverviewViewModel
				{
					Id = book.BookId,
					ISBN = book.Isbn,
					Title = book.Title,
					Author = book.Author,
					ImagePath = book.ImagePath,
					Description = book.Description
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

		public async Task<bool> IsBookAddedToLibrary(string currentUserId, string id)
		{
			bool added = await dbContext.LibraryBooks.AnyAsync(book => EF.Functions.Like(book.BookId, id)  && EF.Functions.Like(book.UserId, currentUserId));
			return added;
		}
	}
}
