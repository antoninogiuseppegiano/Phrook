using Phrook.Models.InputModels;

namespace Phrook.Models.ViewModels
{
    public class BookListViewModel : IPaginationInfo
    {
        public ListViewModel<BookViewModel> Books { get; set; }
		public BookListInputModel Input { get; set; }
		
		int IPaginationInfo.CurrentPage => Input.Page;
		int IPaginationInfo.TotalResults => Books.TotalCount;
		int IPaginationInfo.ResultsPerPage => Input.Limit;
		string IPaginationInfo.Search => Input.Search;
		string IPaginationInfo.OrderBy => Input.OrderBy;
		bool IPaginationInfo.Ascending => Input.Ascending;
	}
}