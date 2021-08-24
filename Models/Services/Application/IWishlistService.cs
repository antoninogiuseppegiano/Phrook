using System.Threading.Tasks;
using Phrook.Models.InputModels;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
	public interface IWishlistService
	{
		Task<ListViewModel<WishlistViewModel>> GetBooksAsync(string currentUserId, BookListInputModel input);
		Task RemoveBookFromWishlist(string currentUserId, string wishlistId);
		Task AddBookToWishlist(string currentUserId, string bookId);
	}
}