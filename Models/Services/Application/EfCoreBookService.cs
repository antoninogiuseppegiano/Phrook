using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Phrook.Models.Entities;
using Phrook.Models.Exceptions;
using Phrook.Models.InputModels;
using Phrook.Models.Options;
using Phrook.Models.Services.Infrastructure;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
	public class EfCoreBookService : IBookService
	{
		private readonly PhrookDbContext dbContext;

		private readonly IOptionsMonitor<BooksOptions> booksOptions;
		private readonly ILogger<EfCoreBookService> logger;

		public EfCoreBookService(PhrookDbContext dbContext, IOptionsMonitor<BooksOptions> booksOptions, ILogger<EfCoreBookService> logger)
		{
			this.logger = logger;
			this.booksOptions = booksOptions;
			this.dbContext = dbContext;
		}

		public async Task<BookDetailViewModel> GetBookByIdAsync(string id)
		{
			logger.LogInformation("Book id: {id} requested.", id);
			BookDetailViewModel book;
			try {
				book = await dbContext.Books
				.AsNoTracking()
				.Where(book => book.Id.Equals(id))
				.Select(book =>
				new BookDetailViewModel
				{
					Id = book.Id,
					ISBN = book.Isbn,
					Title = book.Title,
					Author = book.Author,
					ImagePath = book.ImagePath,
					Rating = book.Rating,
					Tag = book.Tag,
					ReadingState = book.ReadingState,
					Description = book.Description
				})
				.SingleAsync();
			}
			catch (InvalidOperationException) {
				logger.LogWarning("Book {id} not found", id);
				throw new BookNotFoundException(id);
			}

			return book;
		}
		
		public async Task<ListViewModel<BookViewModel>> GetBooksAsync(BookListInputModel model)
		{
			logger.LogInformation("Book list requested.");

			//Ricerca
			IQueryable<Book> baseQuery = dbContext.Books;

			switch(model.OrderBy) {
				case "Title":
					if(model.Ascending) { baseQuery = baseQuery.OrderBy(book => book.Title); }
					else { baseQuery = baseQuery.OrderByDescending(book => book.Title); }
					break;
				case "Author":
					if(model.Ascending) { baseQuery = baseQuery.OrderBy(book => book.Author); }
					else { baseQuery = baseQuery.OrderByDescending(book => book.Author); }
					break;
				case "ReadingState":
					if(model.Ascending) { baseQuery = baseQuery.OrderBy(book => book.ReadingState); }
					else { baseQuery = baseQuery.OrderByDescending(book => book.ReadingState); }
					break;
				case "Rating":
					if(model.Ascending) { baseQuery = baseQuery.OrderBy(book => book.Rating); }
					else { baseQuery = baseQuery.OrderByDescending(book => book.Rating); }
					break;
				case "Tag":
					if(model.Ascending) { baseQuery = baseQuery.OrderBy(book => book.Tag); }
					else { baseQuery = baseQuery.OrderByDescending(book => book.Tag); }
					break;
			}

			IQueryable<BookViewModel> query = baseQuery
			.AsNoTracking()
			//TODO: implementare fuzzy
			.Where(book => book.Title.Contains(model.Search))
			.Select(book =>
			new BookViewModel
			{
				Id = book.Id,
				ISBN = book.Isbn,
				Title = book.Title,
				Author = book.Author,
				ImagePath = book.ImagePath,
				Rating = book.Rating,
				Tag = book.Tag,
				ReadingState = book.ReadingState
			});

			List<BookViewModel> books = await query
			.Skip(model.Offset)
			.Take(model.Limit)
			.ToListAsync();

			int totalCount = await query.CountAsync();
			ListViewModel<BookViewModel> result = new ListViewModel<BookViewModel>{
				Results = books,
				TotalCount = totalCount
			};
			return result;
		}

		
		public async Task<BookDetailViewModel> GetBookByISBNAsync(string ISBN)
		{
			logger.LogInformation("Book id: {ISBN} requested.", ISBN);
			BookDetailViewModel book;
			try {
				book = await dbContext.Books
				.AsNoTracking()
				.Where(book => book.Isbn.Equals(ISBN))
				.Select(book =>
				new BookDetailViewModel
				{
					Id = book.Id,
					ISBN = book.Isbn,
					Title = book.Title,
					Author = book.Author,
					ImagePath = book.ImagePath,
					Rating = book.Rating,
					Tag = book.Tag,
					ReadingState = book.ReadingState,
					Description = book.Description
				})
				.SingleAsync();
			}
			catch (InvalidOperationException) {
				logger.LogWarning("Book {ISBN} not found", ISBN);
				throw new BookNotFoundException(ISBN);
			}

			return book;
		}
	}
}
