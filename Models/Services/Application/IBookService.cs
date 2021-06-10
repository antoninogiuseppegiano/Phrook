using System.Collections.Generic;
using Phrook.Models.ViewModels;

namespace Phrook.Models.Services.Application
{
    public interface IBookService
    {
         List<BookViewModel> GetBooks();
         BookDetailViewModel GetBook(string isbn);
    }
}