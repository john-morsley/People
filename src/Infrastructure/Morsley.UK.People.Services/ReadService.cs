namespace Morsley.UK.People.Services;

public class ReadService : IReadService
{
    private readonly IPersonRepository _repository;
    private readonly ILogger _logger;

    public ReadService(IPersonRepository repository, ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IPagedList<Person> GetPeople(int pageNumber, int pageSize, string? fields, string? filter, string? search, string sort)
    {
        throw new NotImplementedException();
    }

    public Person? GetPerson(Guid requestId)
    {
        throw new NotImplementedException();
    }
}
