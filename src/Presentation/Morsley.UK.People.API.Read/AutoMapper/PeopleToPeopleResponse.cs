namespace Morsley.UK.People.API.Read.AutoMapper
{
    /// <summary>
    /// 
    /// </summary>
    public class PeopleToPeopleResponse : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public PeopleToPeopleResponse()
        {
            CreateMap<DateTime?, string?>().ConvertUsing<NullableDateTimeTypeConverter>();
            CreateMap< Morsley.UK.People.Application.Models.PagedList<Person>, Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse>>().ConvertUsing<PagedListTypeConverter>();
        }
    }
}