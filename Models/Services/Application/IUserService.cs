using System.Threading.Tasks;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
    public interface IUserService
    {
        Task<ListViewModel<SearchedUserViewModel>> GetUsers(string fullname);
    }
}