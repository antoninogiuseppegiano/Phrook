using System.Collections.Generic;
using System.Threading.Tasks;
using Phrook.Models.InputModels;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
    public interface IBookService
    {
        Task<ListViewModel<BookViewModel>> GetBooksAsync(BookListInputModel model);
        Task<BookDetailViewModel> GetBookAsync(int id);
	}
}