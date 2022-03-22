namespace Morsley.UK.People.Application.AutoMapper
{
    public class GetPeopleQueryToGetOptions : Profile
    {
        public GetPeopleQueryToGetOptions()
        {
            CreateMap<GetPeopleQuery, GetOptions>().ConvertUsing(new GetPeopleQueryToGetOptionsConverter());
        }
    }
}
