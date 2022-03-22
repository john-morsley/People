namespace Morsley.UK.People.API.Write.AutoMapper;

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
        //CreateMap<string?, Domain.Enumerations.Sex?>().ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>().ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<DateTime?, string?>().ConvertUsing(new DateTimeToStringConverter());

        CreateMap<Person, PersonResponse>();
    }
}
