namespace Morsley.UK.People.Application.Handlers;

public sealed class AddPersonCommandHandler : IRequestHandler<AddPersonCommand, Person>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;
    private readonly IEventBus _bus;
    private readonly ILogger _logger;
    private readonly ActivitySource _source;

    public AddPersonCommandHandler(
        IPersonRepository personRepository, 
        IMapper mapper, 
        IEventBus bus,
        ILogger logger,
        ActivitySource source)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public async Task<Person> Handle(AddPersonCommand command, CancellationToken ct)
    {
        var name = $"{nameof(AddPersonCommandHandler)}->{nameof(Handle)}";
        _logger.Debug(name);
        using var activity = _source.StartActivity(name, ActivityKind.Server);

        if (command == null) throw new ArgumentNullException(nameof(command));

        var person = _mapper.Map<Person>(command);

        await _personRepository.AddAsync(person);

        RaisePersonAddedEvent(person);

        return person;
    }

    private void RaisePersonAddedEvent(Person person)
    {
        var name = $"{nameof(AddPersonCommandHandler)}->{nameof(RaisePersonAddedEvent)}";
        _logger.Debug(name);
        using var activity = _source.StartActivity(name, ActivityKind.Server);

        var personAddedEvent = _mapper.Map<PersonAddedEvent>(person!);
        _bus.Publish(personAddedEvent);
    }
}