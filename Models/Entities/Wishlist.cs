namespace Phrook.Models.Entities
{
    public class Wishlist
    {
      	public int Id { get; set; }
		public string UserId {get; set;}
		public virtual ApplicationUser User { get; set; }
		public string BookId {get; set;}
		public string Isbn { get; set; }
		public string Title { get; set; }
		public string NormalizedTitle { get; set; }
		public string ImagePath { get; set; }
		public string Author { get; set; }
    }
}