namespace Morsley.UK.People.Application.Handlers;

public class PersonAddedEventHandler : IEventHandler<PersonAddedEvent>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;

    public PersonAddedEventHandler(IPersonRepository personRepository, IMapper mapper)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task Handle(PersonAddedEvent @event)
    {
        if (@event == null) throw new ArgumentNullException(nameof(@event));

        var command = new AddPersonCommand();

        var person = _mapper.Map<Person>(command);

        await _personRepository.AddAsync(person);
    }
}
