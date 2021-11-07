using System;

namespace Users.Domain.Exceptions
{
    public class DomainModelIdException : Exception
    {
        public DomainModelIdException(string message) : base(message) { }
    }
}