using System.Collections.Generic;
using Phrook.Models.InputModels;

namespace Phrook.Models.ViewModels
{
    public class BookListViewModel
    {
        public ListViewModel<BookViewModel> Books { get; set; }
		public BookListInputModel Input { get; set; }
    }
}