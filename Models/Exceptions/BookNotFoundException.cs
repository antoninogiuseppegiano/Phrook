using System;

namespace Phrook.Models.Exceptions
{
    public class BookNotFoundException : Exception
    {
		public BookNotFoundException()
		{
		}

		public BookNotFoundException(int id) : base($"Book {id} not found"){}
        public BookNotFoundException(string ISBN) : base($"Book {ISBN} not found"){}
    }
}