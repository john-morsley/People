namespace Morsley.UK.People.Domain.Exceptions;

public class DomainModelIdException : Exception
{
    public DomainModelIdException(string message) : base(message) { }
}