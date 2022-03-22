namespace Morsley.UK.People.API.Read.AutoMapper
{
    /// <summary>
    /// 
    /// </summary>
    public class PersonToPersonResponse : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public PersonToPersonResponse()
        {
            CreateMap<Person, PersonResponse>();
        }
    }
}
