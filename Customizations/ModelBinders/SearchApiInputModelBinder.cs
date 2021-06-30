using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Phrook.Models.InputModels;
using Phrook.Models.Options;

namespace Phrook.Customizations.ModelBinders
{
    public class SearchApiInputModelBinder : IModelBinder
    {
		private readonly IOptionsMonitor<BooksOptions> booksOptions;

		public SearchApiInputModelBinder(IOptionsMonitor<BooksOptions> booksOptions)
		{
			this.booksOptions = booksOptions;
		}
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			//retrieving values
			string orderBy = bindingContext.ValueProvider.GetValue("OrderBy").FirstValue;
			int.TryParse(bindingContext.ValueProvider.GetValue("Page").FirstValue, out int page);
			bool.TryParse(bindingContext.ValueProvider.GetValue("Ascending").FirstValue, out bool ascending);
			BooksOptions options = booksOptions.CurrentValue;

			string searchISBN = bindingContext.ValueProvider.GetValue("SearchISBN").FirstValue;
			
			//creating input model
			SearchApiInputModel inputModel;
			if(searchISBN is null) 
			{
				string searchTitle = bindingContext.ValueProvider.GetValue("SearchTitle").FirstValue;
				string searchAuthor = bindingContext.ValueProvider.GetValue("SearchAuthor").FirstValue;
				inputModel = new SearchApiInputModel(searchTitle, searchAuthor, page, orderBy, ascending, options.PerPage, options.Order);
			}
			else
			{
				inputModel = new SearchApiInputModel(searchISBN, page, orderBy, ascending, options.PerPage, options.Order);
			}
			// 

			//successful binding
			bindingContext.Result = ModelBindingResult.Success(inputModel);
			return Task.CompletedTask;
		}
    }
}