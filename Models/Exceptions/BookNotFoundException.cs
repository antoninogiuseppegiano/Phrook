using System;

namespace Phrook.Models.Exceptions
{
    public class BookNotFoundException : Exception
    {
        public BookNotFoundException(int id) : base($"Book {id} not found"){}
    }
}