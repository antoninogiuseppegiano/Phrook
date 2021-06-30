using Phrook.Models.InputModels;

namespace Phrook.Models.ViewModels
{
    public class SearchBookListViewModel
    {
        public ListViewModel<SearchedBookViewModel> Books { get; set; }
		public SearchApiInputModel Input { get; set; }
    }
}