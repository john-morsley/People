namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class UpdatePersonRequestToAddPersonRequest : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public UpdatePersonRequestToAddPersonRequest()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<UpdatePersonRequest, AddPersonRequest>();
    }
}
