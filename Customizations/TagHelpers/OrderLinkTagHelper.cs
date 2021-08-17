using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Phrook.Models.InputModels;

namespace Phrook.Customizations.TagHelpers
{
    public class OrderLinkTagHelper : AnchorTagHelper
    {
        public string OrderBy { get; set; }
        public BookListInputModel Input { get; set; }
		public string UserId { get; set; }

        public OrderLinkTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            //Imposto i valori del link
            RouteValues["search"] = Input.Search;
            RouteValues["orderby"] = OrderBy;
            RouteValues["ascending"] = (Input.OrderBy == OrderBy ? !Input.Ascending : Input.Ascending).ToString().ToLowerInvariant();
            if(!string.IsNullOrWhiteSpace(UserId))
			{
				RouteValues["userId"] = UserId;
			}

            //Faccio generare l'output all'AnchorTagHelper
            base.Process(context, output);

            //Aggiungo l'indicatore di direzione
            if (Input.OrderBy == OrderBy)
            {
                var direction = Input.Ascending ? "up" : "down";
                output.PostContent.SetHtmlContent($" <i class=\"fas fa-caret-{direction}\"></i>");
            }
        }
    }
}