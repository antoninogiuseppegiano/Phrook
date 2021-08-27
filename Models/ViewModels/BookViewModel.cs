namespace Phrook.Models.ViewModels
{
    public class BookViewModel
    {
        public string Id { get; set; }
		public string ISBN { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string ImagePath { get; set; }
		public double Rating { get; set; }
		public string Tag { get; set; }
		public string ReadingState { get; set; }
    }
}