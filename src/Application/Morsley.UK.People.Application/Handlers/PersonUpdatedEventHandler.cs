namespace Morsley.UK.People.Application.Handlers;

public class PersonUpdatedEventHandler : IEventHandler<PersonUpdatedEvent>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;

    public PersonUpdatedEventHandler(IPersonRepository personRepository, IMapper mapper)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task Handle(PersonUpdatedEvent personUpdatedEvent)
    {
        if (personUpdatedEvent == null) throw new ArgumentNullException(nameof(personUpdatedEvent));

        try
        {
            var person = _mapper.Map<Person>(personUpdatedEvent);
            await _personRepository.UpdateAsync(person);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
