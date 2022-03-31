namespace Morsley.UK.People.Application.AutoMapper;

public class PersonToPersonAddedEvent : Profile
{
    public PersonToPersonAddedEvent()
    {
        CreateMap<Person, PersonAddedEvent>();
    }
}