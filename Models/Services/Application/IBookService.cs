using System.Collections.Generic;
using System.Threading.Tasks;
using Phrook.Models.InputModels;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
    public interface IBookService
    {
        Task<ListViewModel<BookViewModel>> GetBooksAsync(string currentUserId, BookListInputModel model);
        Task<BookDetailViewModel> GetBookByIdAsync(string currentUserId, string id);
		Task<BookDetailViewModel> GetBookByISBNAsync(string currentUserId, string ISBN);
		Task<BookDetailViewModel> EditBookAsync(string currentUserId, EditBookInputModel inputModel);
		Task<EditBookInputModel> GetBookForEditingAsync(string currentUserId, string id);
		Task RemoveBookFromLibrary(string currentUserId, string id);
		Task AddBookToLibrary(string currentUserId, string id);
		Task<bool> IsBookStoredInBooks(string id);
		Task<bool> IsBookInWishList(string currentUserId, string bookId);
		Task<BookOverviewViewModel> GetBookNotAddedInLibaryByIdAsync(string id);
		Task<bool> IsBookAddedToLibrary(string currentUserId, string id);
	}
}