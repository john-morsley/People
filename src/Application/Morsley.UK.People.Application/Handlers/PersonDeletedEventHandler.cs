namespace Morsley.UK.People.Application.Handlers;

public class PersonDeletedEventHandler : IEventHandler<PersonDeletedEvent>
{
    private readonly IPersonRepository _personRepository;

    public PersonDeletedEventHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task Handle(PersonDeletedEvent personDeletedEvent)
    {
        if (personDeletedEvent == null) throw new ArgumentNullException(nameof(personDeletedEvent));

        try
        {
            await _personRepository.DeleteAsync(personDeletedEvent.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
