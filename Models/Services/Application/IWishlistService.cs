using System.Threading.Tasks;
using Phrook.Models.InputModels;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
	public interface IWishlistService
	{
		Task<ListViewModel<WishlistViewModel>> GetBooksAsync(BookListInputModel input);
		Task RemoveBookFromWishlist(string wishlistId);
		Task AddBookToWishlist(string bookId);
	}
}