namespace Morsley.UK.People.Application.AutoMapper
{
    public class PartiallyUpdatePersonCommandToPerson : Profile
    {
        public PartiallyUpdatePersonCommandToPerson()
        {
            CreateMap<PartiallyUpdatePersonCommand, Person>();
        }
    }
}