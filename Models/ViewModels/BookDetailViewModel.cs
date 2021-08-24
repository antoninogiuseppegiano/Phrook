namespace Phrook.Models.ViewModels
{
    public class BookDetailViewModel : BookViewModel
    {
        public string Description { get; set; }
		public string InitialTime { get; set; }
		public string FinalTime { get; set; }
    }
}