namespace Morsley.UK.People.Application.Queries;

public sealed class GetPersonQuery : Cacheable, IRequest<Person>
{
    public Guid Id { get; set; }

    public override string ToString()
    {
        return $"Id:{Id}";
    }

    public override string CacheKey => Person.GetCacheKey(Id);
}