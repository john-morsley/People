namespace Morsley.UK.People.Application.Handlers;

public sealed class AddPersonCommandHandler : IRequestHandler<AddPersonCommand, Person>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;
    private readonly IEventBus _bus;

    public AddPersonCommandHandler(IPersonRepository personRepository, IMapper mapper, IEventBus bus)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _bus = bus;
    }

    public async Task<Person> Handle(AddPersonCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var person = _mapper.Map<Person>(command);

        await _personRepository.AddAsync(person);

        var personAddedEvent = _mapper.Map<PersonAddedEvent>(person!);

        //var personAddedEvent = new PersonAddedEvent();

        _bus.Publish(personAddedEvent);

        return person;
    }
}