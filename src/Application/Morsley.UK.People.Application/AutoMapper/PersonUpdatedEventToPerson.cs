namespace Morsley.UK.People.Application.AutoMapper;

public class PersonUpdatedEventToPerson : Profile
{
    public PersonUpdatedEventToPerson()
    {
        CreateMap<PersonUpdatedEvent, Person>();
    }
}