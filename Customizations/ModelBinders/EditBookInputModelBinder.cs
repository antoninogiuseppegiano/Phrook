using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Phrook.Models.InputModels;

namespace Phrook.Customizations.ModelBinders
{
    public class EditBookInputModelBinder
    {
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			//retrieving values
			string title = bindingContext.ValueProvider.GetValue("Title").FirstValue;
			int.TryParse(bindingContext.ValueProvider.GetValue("Id").FirstValue, out int id);

			string tag = bindingContext.ValueProvider.GetValue("Tag").FirstValue;
			string readingState = bindingContext.ValueProvider.GetValue("ReadingState").FirstValue;
			double.TryParse(bindingContext.ValueProvider.GetValue("Rating").FirstValue, out double rating);

			//creating input model
			// EditBookInputModel inputModel = new EditBookInputModel(id, title, rating, tag, readingState);

			//successful binding
			// bindingContext.Result = ModelBindingResult.Success(inputModel);
			return Task.CompletedTask;
		}
    }
}