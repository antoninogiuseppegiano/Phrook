using System;

namespace Phrook.Models.Exceptions
{
    public class UserUnknownException : Exception
    {
        public UserUnknownException() : base($"A known user is required for this operation"){}
    }
}