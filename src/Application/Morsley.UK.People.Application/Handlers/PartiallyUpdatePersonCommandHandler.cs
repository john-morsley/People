namespace Morsley.UK.People.Application.Handlers;

public sealed class PartiallyUpdatePersonCommandHandler : IRequestHandler<PartiallyUpdatePersonCommand, Person>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;
    private readonly IEventBus _bus;

    public PartiallyUpdatePersonCommandHandler(
        IPersonRepository personRepository,
        IMapper mapper,
        IEventBus bus)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    public async Task<Person> Handle(PartiallyUpdatePersonCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var person = _mapper.Map<Person>(command);

        await _personRepository.UpdateAsync(person);

        RaisePersonUpdatedEvent(person);

        return person;
    }

    private void RaisePersonUpdatedEvent(Person person)
    {
        var personUpdatedEvent = _mapper.Map<PersonUpdatedEvent>(person!);
        _bus.Publish(personUpdatedEvent);
    }
}