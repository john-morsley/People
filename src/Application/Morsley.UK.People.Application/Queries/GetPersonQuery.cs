namespace Morsley.UK.People.Application.Queries;

public sealed class GetPersonQuery : IRequest<Person>
{
    public Guid Id { get; set; }

    public override string ToString()
    {
        return $"Id:{Id}";
    }

}