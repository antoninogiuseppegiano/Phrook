
using System;
using Phrook.Models.Enums;
using Phrook.Models.Services.Application;

#nullable disable

namespace Phrook.Models.Entities
{
	public partial class LibraryBook
	{
		public LibraryBook(int bookId, string userId)
        {
			//TODO: validazione bookId e userId
		    /* if ()
            {
                throw new ArgumentException("Il libro non esiste"); //BookNotFoundException
            }
			if ()
            {
                throw new ArgumentException("L'utente non esiste");
            }  */  
            BookId = bookId;
            UserId = userId;
        }
		public int Id { get; private set; }
		public virtual Book Book { get; set; }
        public int BookId { get; set; }
		public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }
		public double Rating { get; private set; }
        public string Tag { get; private set; }
        public string ReadingState { get; private set; }
        public string RowVersion { get; private set; }
		
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