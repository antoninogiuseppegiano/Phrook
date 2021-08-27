using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Phrook.Models.InputModels;
using Phrook.Models.Options;

namespace Phrook.Customizations.ModelBinders
{
	public class BookListInputModelBinder : IModelBinder
	{
		private readonly IOptionsMonitor<BooksOptions> booksOptions;

		public BookListInputModelBinder(IOptionsMonitor<BooksOptions> booksOptions)
		{
			this.booksOptions = booksOptions;
		}
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			//retrieving values
			string search = bindingContext.ValueProvider.GetValue("Search").FirstValue;
            string orderBy = bindingContext.ValueProvider.GetValue("OrderBy").FirstValue;
            int.TryParse(bindingContext.ValueProvider.GetValue("Page").FirstValue, out int page);
            bool.TryParse(bindingContext.ValueProvider.GetValue("Ascending").FirstValue, out bool ascending);

			//creating input model
			BooksOptions options = booksOptions.CurrentValue;
			BookListInputModel inputModel = new (search, page, orderBy, ascending, options.PerPage, options.Order);

			//successful binding
			bindingContext.Result = ModelBindingResult.Success(inputModel);
			return Task.CompletedTask;
			
		}
	}
}