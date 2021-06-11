using System.Collections.Generic;
using System.Threading.Tasks;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
    public interface IBookService
    {
        Task<List<BookViewModel>> GetBooksAsync();
        Task<BookDetailViewModel> GetBookAsync(int id);
	}
}