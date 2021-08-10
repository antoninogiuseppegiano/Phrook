using Microsoft.AspNetCore.Mvc;
using Phrook.Models.ViewModels;

namespace Phrook.Customizations.ViewComponents
{
    public class PaginationBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IPaginationInfo model) 
		{
			return View(model);
		}
    }
}