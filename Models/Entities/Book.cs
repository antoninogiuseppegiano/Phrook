using System;
using System.Collections.Generic;

#nullable disable

namespace Phrook.Models.Entities
{
    public partial class Book
    {
		public Book(string title, string author)
        {
		    if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("The course must have a title");
            }
			if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("The course must have an author");
            }   
            Title = title;
            Author = author;
        }

        public int Id { get; private set; }
        public string Isbn { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Author { get; private set; }
        public double Rating { get; private set; }
        public string Tag { get; private set; }
        public string ReadingState { get; private set; }

		// ICollection<Review> Reviews { get; set; }

		public void ChangeTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
            {
                throw new ArgumentException("The course must have a title");
            }
            Title = newTitle;
        }
    }
}
