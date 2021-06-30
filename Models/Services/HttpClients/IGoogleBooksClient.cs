using System.Threading.Tasks;
using Phrook.Models.ResponseModels;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.HttpClients
{
    public interface IGoogleBooksClient
    {
        Task<ListViewModel<SearchedBookViewModel>> GetBooksByTitleAuthorAsync(string title, string author);
        Task<string> GetIdFromISBNAsync(string isbn);
        Task<BookOverviewViewModel> GetBookByIdAsync(string id);
    }
}