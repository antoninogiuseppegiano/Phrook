using System;

namespace Phrook.Models.Exceptions
{
    public class ApiException : Exception
    {
		//value stnads for id or isbn
        public ApiException(string value) : base($"Book {value} not found"){}
        public ApiException(string title, string author) : base($"Book {(string.IsNullOrWhiteSpace(title) ? "" : title + " ")}{(string.IsNullOrWhiteSpace(author) ? "" : "(" + author + ") ")}not found"){}
    }
}