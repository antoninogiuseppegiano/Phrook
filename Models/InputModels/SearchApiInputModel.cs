using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Phrook.Customizations.ModelBinders;
using Phrook.Models.Options;

namespace Phrook.Models.InputModels
{
	[ModelBinder(BinderType = typeof(SearchApiInputModelBinder))]
    public class SearchApiInputModel
    {
		public SearchApiInputModel(string searchISBN, int page, string orderBy, bool ascending, int limit, BooksOrderOptions orderOptions)
		{
			if(!orderOptions.Allow.Contains(orderBy)) 
			{
				//order not specified, stting default
				orderBy = orderOptions.By;
				ascending = orderOptions.Ascending;
			}
			
			this.SearchISBN = searchISBN.Trim() ?? "";
			SearchTitle = null;
			SearchAuthor = null;
			this.Page = Math.Max(1, page);
			this.OrderBy = orderBy;
			this.Ascending = ascending;

			this.Limit = Math.Max(1, limit);
			this.Offset = (Page - 1) * this.Limit;
		}

		public SearchApiInputModel(string searchTitle, string searchAuthor, int page, string orderBy, bool ascending, int limit, BooksOrderOptions orderOptions)
		{
			if(!orderOptions.Allow.Contains(orderBy)) 
			{
				//order not specified, stting default
				orderBy = orderOptions.By;
				ascending = orderOptions.Ascending;
			}
			
			this.SearchTitle= searchTitle.Trim() ?? "";
			this.SearchAuthor = searchAuthor.Trim() ?? "";
			SearchISBN = null;
			this.Page = Math.Max(1, page);
			this.OrderBy = orderBy;
			this.Ascending = ascending;

			this.Limit = Math.Max(1, limit);
			this.Offset = (Page - 1) * this.Limit;
		}
		public string SearchISBN { get; }
		public string SearchTitle { get; }
		public string SearchAuthor { get; }
		public int Page { get; }
		public string OrderBy { get; }
		public bool Ascending { get; }
		public int Limit { get; }
		public int Offset { get; }        
    }
}