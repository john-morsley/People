namespace Morsley.UK.People.Application.Handlers;

public sealed class GetPageOfPeopleQueryHandler : IRequestHandler<GetPeopleQuery, PagedList<Person>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;
    private readonly ActivitySource _source;

    public GetPageOfPeopleQueryHandler(
        IPersonRepository personRepository,
        IMapper mapper,
        ActivitySource source)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public async Task<PagedList<Person>> Handle(GetPeopleQuery query, CancellationToken ct)
    {
        var name = $"GetPageOfPeopleQueryHandler->{nameof(Handle)}";
        using var activity = _source.StartActivity(name, ActivityKind.Server);

        if (query == null) throw new ArgumentNullException(nameof(query));

        var getOptions = _mapper.Map<GetOptions>(query);

        var people = await _personRepository.GetPageAsync(getOptions);

        return people as PagedList<Person>;
    }
}