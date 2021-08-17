using System.Collections.Generic;

namespace Phrook.Models.Entities
{
    public class Wishlist
    {
      	public int Id { get; private set; }
		public string UserId {get; private set;}
		public virtual ApplicationUser User { get; set; }
		public string BookId {get; private set;}
		public string Isbn { get; private set; }
		public string Title { get; private set; }
		public string NormalizedTitle { get; private set; }
		public string ImagePath { get; private set; }
		public string Author { get; private set; }
    }
}