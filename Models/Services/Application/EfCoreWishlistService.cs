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
using Phrook.Models.Services.HttpClients;
using Phrook.Models.Services.Infrastructure;
using Phrook.Models.ViewModels;
using Z.EntityFramework.Plus;

namespace Phrook.Models.Services.Application
{
	public class EfCoreWishlistService : IWishlistService
	{
		private readonly PhrookDbContext dbContext;
		private readonly IGoogleBooksClient _gbClient;
		private readonly IBookService bookService;
		public EfCoreWishlistService(IGoogleBooksClient googleBooksClient, IBookService bookService, PhrookDbContext dbContext)
		{
			this.bookService = bookService;
			this._gbClient = googleBooksClient;
			this.dbContext = dbContext;
		}
		public async Task<ListViewModel<WishlistViewModel>> GetBooksAsync(string currentUserId, BookListInputModel input)
		{
			
			IQueryable<WishlistViewModel> query = dbContext.Wishlist
			.AsNoTracking()
			//TODO: implementare fuzzy
			.Where(wishlist => wishlist.UserId == currentUserId)
			.Select(wishlist =>
			new WishlistViewModel
			{
				// Id = wishlist.Id,
				BookId = wishlist.BookId,
				Isbn = wishlist.Isbn,
				Title = wishlist.Title,
				Author = wishlist.Author,
				ImagePath = wishlist.ImagePath
			});

			List<WishlistViewModel> books = await query.ToListAsync();

			int totalCount = await query.CountAsync();
			ListViewModel<WishlistViewModel> result = new()
			{
				Results = books,
				TotalCount = totalCount
			};
			return result;
		}

		public async Task RemoveBookFromWishlist(string currentUserId, string bookId)
		{
			int affectedRows = await dbContext.Wishlist.Where(wishlist => wishlist.BookId == bookId && wishlist.UserId == currentUserId).DeleteAsync();
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
		}

		public async Task AddBookToWishlist(string currentUserId, string bookId)
		{
			bool isBookAddedInLibrary = await dbContext.LibraryBooks.Where(l => l.UserId == currentUserId && l.BookId == bookId).CountAsync() > 0;
			if(isBookAddedInLibrary)
			{
				throw new BookNotAddedException(bookId);
			}

			Wishlist wishlist;
			if (!(await bookService.IsBookStoredInBooks(bookId)))
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

				wishlist = new()
				{
					UserId = currentUserId,
					BookId = bookId,
					Isbn = overview.ISBN,
					Title = overview.Title,
					NormalizedTitle = overview.Title.ToLower(),
					ImagePath = overview.ImagePath,
					Author = overview.Author
				};
			}
			else
			{
				Book book = await dbContext.Books.Where(book => book.BookId == bookId).SingleAsync();
				wishlist = new()
				{
					UserId = currentUserId,
					BookId = bookId,
					Isbn = book.Isbn,
					Title = book.Title,
					NormalizedTitle = book.Title.ToLower(),
					ImagePath = book.ImagePath,
					Author = book.Author
				};
			}

			dbContext.Add(wishlist);

			try
			{
				await dbContext.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				throw new BookNotAddedException(bookId);
			}

		}
	}
}