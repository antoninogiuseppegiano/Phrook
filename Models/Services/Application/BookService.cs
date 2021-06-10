using System;
using System.Collections.Generic;
using Phrook.Models.Enums;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
	public class BookService : IBookService
	{
		public BookDetailViewModel GetBook(string isbn)
		{
			var rand = new Random();
			var book = new BookDetailViewModel
			{
				Id = Convert.ToInt32(isbn),
				ISBN = $"{isbn}",
				Title = $"Libro {isbn}",
				Author = $"Autore Tizio",
				ImagePath = "/logo-phrook-color.png",
				Rating = rand.Next(10, 50)/10.0,
				Tag = Tag.Fantascienza.ToString(),
				ReadingState = ReadingState.Interrupted.ToString(),
				Description = "That's a long story"
			};
			return book;
		}

		public List<BookViewModel> GetBooks()
		{
			var books = new List<BookViewModel>();
			var rand = new Random();
			for (int i = 1; i <= 20; i++)
            {
				var book = new BookViewModel
				{
					Id = i,
					ISBN = $"{i}{i}{i}{i}",
					Title = $"Libro {i}",
					Author = $"Autore Tizio",
					ImagePath = "/logo-phrook-color.png",
					Rating = rand.Next(10, 50)/10.0,
					Tag = Tag.Fantascienza.ToString(),
					ReadingState = ReadingState.Interrupted.ToString()
				};
				books.Add(book);
			}
			return books;
		}
	}
}