using System;

namespace Phrook.Models.Exceptions
{
    public class BookNotAddedException : Exception
    {
        public BookNotAddedException(string id) : base($"Couldn't add book {id}")
        {
        }
    }
}