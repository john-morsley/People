namespace Morsley.UK.People.Application.AutoMapper;

public class AddPersonCommandToPerson : Profile
{
    public AddPersonCommandToPerson()
    {
        CreateMap<AddPersonCommand, Person>().ConvertUsing(new AddPersonComandToPersonConverter());
    }
}