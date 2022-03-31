namespace Morsley.UK.People.Application.AutoMapper;

public class PersonAddedEventToPerson : Profile
{
    public PersonAddedEventToPerson()
    {
        CreateMap<PersonAddedEvent, Person>();
    }
}