using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Phrook.Customizations.ModelBinders;
using Phrook.Models.Options;

namespace Phrook.Models.InputModels
{
	[ModelBinder(BinderType = typeof(BookListInputModelBinder))]
	public class BookListInputModel
	{
		public BookListInputModel(string search, int page, string orderBy, bool ascending, int limit, BooksOrderOptions orderOptions)
		{
			if(!orderOptions.Allow.Contains(orderBy)) 
			{
				//order not specified, stting default
				orderBy = orderOptions.By;
				ascending = orderOptions.Ascending;
			}

			this.Search = search ?? "";
			this.Page = Math.Max(1, page);
			this.OrderBy = orderBy;
			this.Ascending = ascending;

			this.Limit = Math.Max(1, limit);
			this.Offset = (Page - 1) * this.Limit;
		}
		public string Search { get; set; }
		public int Page { get; set; }
		public string OrderBy { get; }
		public bool Ascending { get; }
		public int Limit { get; }
		public int Offset { get; set; }

	}
}