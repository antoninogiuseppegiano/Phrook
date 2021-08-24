using System;

namespace Phrook.Models.Exceptions
{
    public class TooManyRowsException : Exception
    {
        public TooManyRowsException(int rows) : base($"Too many rows: {rows}"){}
    }
}