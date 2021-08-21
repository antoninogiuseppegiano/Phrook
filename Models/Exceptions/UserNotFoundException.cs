using System;

namespace Phrook.Models.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string userId) : base($"User {userId} not found")
        {
        }
    }
}