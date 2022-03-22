namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class PersonToUpdatePersonRequest : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public PersonToUpdatePersonRequest()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>().ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>().ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Person, UpdatePersonRequest>();
    }
}
