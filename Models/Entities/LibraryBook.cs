
using System;
using Phrook.Customizations.ExtensionMethods;
using Phrook.Models.Enums;

#nullable disable

namespace Phrook.Models.Entities
{
	public partial class LibraryBook
	{
		public LibraryBook(string bookId, string userId)
        {
		    if (string.IsNullOrWhiteSpace(bookId))
            {
                throw new ArgumentException("Il libro non esiste"); //BookNotFoundException
            }
			if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("L'utente non esiste");
            } 
            BookId = bookId;
            UserId = userId;
			Rating = 0;
			ReadingState = Enums.ReadingState.NotRead.GetDescription();
        }

		public int Id { get; private set; }
		public virtual Book Book { get; set; }
        public string BookId { get; set; }
		public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }
		public double Rating { get; private set; }
        public string Tag { get; private set; }
        public string ReadingState { get; private set; }
        public string InitialTime { get; private set; }
        public string FinalTime { get; private set; }
        public string RowVersion { get; private set; }
		
		public void ChangeRating(double newRating)
        {
            if (newRating < 0 || newRating > 5)
            {
                throw new ArgumentException("The rating must be between 0 and 5.");
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
            Tag = ((Tag)newTagIndex).GetDescription();
        }

		public void ChangeReadingState(string s_newReadingStateIndex)
        {
			int.TryParse(s_newReadingStateIndex, out int newReadingStateIndex);
            if (!Enum.IsDefined(typeof(ReadingState), newReadingStateIndex))
            {
                throw new ArgumentException("This reading state value isn't valid.");
            }
            ReadingState = ((ReadingState)newReadingStateIndex).GetDescription();
        }
		
		public void ChangeInitialTime(DateTime time)
        {
			if(time.Date > DateTime.Now.Date  || time.Year < 1900)
            {
                // throw new ArgumentException("This initial time value isn't valid.");
				InitialTime = DateTime.MinValue.Date.ToString("yyyy-MM-dd");
				return;
            }

            InitialTime = time.Date.ToString("yyyy-MM-dd");
			CheckFinalTimeAfterInitialTime();
        }

		public void ChangeFinalTime(DateTime time)
        {
			if(time.Date > DateTime.Now.Date || time.Year < 1900)
            {
                // throw new ArgumentException("This final time value isn't valid.");
				FinalTime = DateTime.MinValue.Date.ToString("yyyy-MM-dd");
				return;
            }

            FinalTime = time.Date.ToString("yyyy-MM-dd");
			
			CheckFinalTimeAfterInitialTime();
        }

		private void CheckFinalTimeAfterInitialTime()
		{
			if(Convert.ToDateTime(InitialTime).Date > Convert.ToDateTime(FinalTime).Date)
            {
				FinalTime = DateTime.MinValue.Date.ToString("yyyy-MM-dd");
            }
		}
	}
}