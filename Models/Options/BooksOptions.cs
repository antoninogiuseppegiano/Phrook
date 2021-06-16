namespace Phrook.Models.Options
{
    public class BooksOptions
    {
	
		public int PerPage { get; set; }
		public BooksOrderOptions Order { get; set; }
	}

	public partial class BooksOrderOptions
	{
		public string By { get; set; }
		public bool Ascending { get; set; }
		public string[] Allow { get; set; }
	}
}