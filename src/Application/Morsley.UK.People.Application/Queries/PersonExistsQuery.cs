namespace Morsley.UK.People.Application.Queries;

public sealed class PersonExistsQuery : IRequest<bool>
{
    public PersonExistsQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public override string ToString()
    {
        return $"Id:{Id}";
    }

}