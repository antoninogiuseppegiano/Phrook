using System;
using System.Collections.Generic;
using Phrook.Models.Enums;
using Phrook.Models;

#nullable disable

namespace Phrook.Models.Entities
{
    public partial class Book
    {
		public Book(string bookId, string isbn, string title, string author, string imagePath, string description)
        {
			if (string.IsNullOrWhiteSpace(isbn))
			{
				throw new ArgumentException($"'{nameof(isbn)}' non può essere null o vuoto.", nameof(isbn));
			}

			if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Il libro deve avere un titolo");
            }
			if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("Il libro deve avere un autore");
            }
			BookId = bookId;
			Isbn = isbn;
			Title = title;
			NormalizedTitle = title.ToLower();
            Author = author;
			ImagePath = imagePath;
			Description = description;
		}

        public int Id { get; private set; }
		public string BookId {get; private set;}
        public string Isbn { get; private set; }
        public string Title { get; private set; }
        public string NormalizedTitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Author { get; private set; }
		public ICollection<LibraryBook> InLibrary { get; set; }
    }
}
