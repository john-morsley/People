namespace Morsley.UK.People.Application.AutoMapper
{
    public class AddPersonCommandToPerson : Profile
    {
        public AddPersonCommandToPerson()
        {
            CreateMap<AddPersonCommand, 
                Person>().ConstructUsing(_ => new Person(_.Id == Guid.Empty ? Guid.NewGuid() : _.Id, _.FirstName, _.LastName));
        }
    }
}
