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

    public async Task Handle(PersonAddedEvent personAddedEvent)
    {
        if (personAddedEvent == null) throw new ArgumentNullException(nameof(personAddedEvent));

        try
        {
            var person = _mapper.Map<Person>(personAddedEvent);
            await _personRepository.AddAsync(person);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
