using System;

namespace Phrook.Models.Exceptions
{
    public class BookNotFoundException : Exception
    {
		public BookNotFoundException()
		{
		}

		public BookNotFoundException(string value) : base($"Book {value} not found"){}
    }
}