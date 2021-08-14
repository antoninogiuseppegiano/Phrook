using System;
using System.Collections.Generic;
using Phrook.Models.Enums;
using Phrook.Models;

#nullable disable

namespace Phrook.Models.Entities
{
    public partial class Book
    {
		public Book(string title, string author)
        {
		    if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Il libro deve avere un titolo");
            }
			if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("Il libro deve avere un autore");
            }   
            Title = title;
            Author = author;
			ImagePath = "/Books/default.png";
        }

        public int Id { get; private set; }
        public string Isbn { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Author { get; private set; }
        #region da eliminare
		public double Rating { get; private set; }
        public string Tag { get; private set; }
        public string ReadingState { get; private set; }
        public string RowVersion { get; private set; }
		#endregion
		// ICollection<Review> Reviews { get; set; }

		public void ChangeRating(double newRating)
        {
            if (newRating < 1 || newRating > 5)
            {
                throw new ArgumentException("The rating must be between 1 and 5.");
            }
            Rating = newRating;
        }

		public void ChangeTag(string s_newTagIndex)
        {
			int.TryParse(s_newTagIndex, out int newTagIndex);
            // if (!Enum.IsDefined(typeof(Tag), newTag))
			if(!Enum.IsDefined(typeof(Tag), newTagIndex))
            {
            	throw new ArgumentException("This tag value isn't valid.");
            }
            Tag = ((Tag)newTagIndex).ToString();
        }

		public void ChangeReadingState(string s_newReadingStateIndex)
        {
			int.TryParse(s_newReadingStateIndex, out int newReadingStateIndex);
            if (!Enum.IsDefined(typeof(ReadingState), newReadingStateIndex))
            {
                throw new ArgumentException("This reading state value isn't valid.");
            }
            ReadingState = ((ReadingState)newReadingStateIndex).ToString();
        }
    }
}
