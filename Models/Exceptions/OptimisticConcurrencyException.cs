using System;

namespace Phrook.Models.Exceptions
{
    public class OptimisticConcurrencyException : Exception
    {
        public OptimisticConcurrencyException() : base($"Row not updated.")
		{
		}
    }
}