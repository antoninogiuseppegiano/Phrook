using Phrook.Models.InputModels;

namespace Phrook.Models.ViewModels
{
    public class SearchUserListViewModel
    {
        public ListViewModel<SearchedUserViewModel> Users { get; set; }
		public string SearchUser { get; set; }
    }
}