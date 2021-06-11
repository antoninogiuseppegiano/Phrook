using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Phrook.Models.Options;
using Phrook.Models.Services.Infrastructure;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
	public class EfCoreBookService : IBookService
	{
		private readonly PhrookDbContext dbContext;

		private readonly IOptionsMonitor<BooksOptions> booksOptions;

		public EfCoreBookService(PhrookDbContext dbContext, IOptionsMonitor<BooksOptions> booksOptions)
		{
			this.booksOptions = booksOptions;
			this.dbContext = dbContext;
		}

		public async Task<BookDetailViewModel> GetBookAsync(int id)
		{
			BookDetailViewModel book = await dbContext.Books
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

			return book;
		}

		public async Task<List<BookViewModel>> GetBooksAsync()
		{
			List<BookViewModel> books = await dbContext.Books
			.AsNoTracking()
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
			})
			.ToListAsync();

			return books;
		}
	}
}
