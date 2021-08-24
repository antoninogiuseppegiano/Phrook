namespace Phrook.Models.ViewModels
{
    public class BookOverviewViewModel
    {
        public string Id { get; set; }
		public string ISBN { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string ImagePath { get; set; }
		public string Description { get; set; }
		public bool IsInLibrary { get; set; }
		public bool IsInWishlist { get; set; }
    }
}