namespace Morsley.UK.People.Application.Handlers;

public sealed class GetPageOfPeopleQueryHandler : IRequestHandler<GetPeopleQuery, IPagedList<Person>>
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

    public async Task<IPagedList<Person>> Handle(GetPeopleQuery query, CancellationToken ct)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        using var activity = _source.StartActivity(name: nameof(GetPageOfPeopleQueryHandler), ActivityKind.Server);

        var getOptions = _mapper.Map<GetOptions>(query);

        var people = await _personRepository.GetPageAsync(getOptions);

        return people;
    }
}