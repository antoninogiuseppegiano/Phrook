using System;

namespace Phrook.Models.Exceptions
{
    public class InvalidApiInputException : Exception
    {
		//value stnads for id or isbn
        public InvalidApiInputException(string value) : base($"Input {value} not valid"){}
        public InvalidApiInputException(string title, string author) : base($"Input title: {{ {title} }} - author: {{ {author} }} not valid"){}
    }
}