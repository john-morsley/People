namespace Morsley.UK.People.Application.AutoMapper;

public class PersonToPersonUpdatedEvent : Profile
{
    public PersonToPersonUpdatedEvent()
    {
        CreateMap<Person, PersonUpdatedEvent>();
    }
}