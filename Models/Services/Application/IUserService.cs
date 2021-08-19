using System.Threading.Tasks;
using Phrook.Models.InputModels;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
    public interface IUserService
    {
        Task<ListViewModel<SearchedUserViewModel>> GetUsers(string currentUserId, string fullname);
		Task<string> GetUserFullName(string userId);
		Task<ListViewModel<BookViewModel>> GetUserBooks(string userId, BookListInputModel input);
		Task<bool> IsVisible(string userId);
		
	}
}