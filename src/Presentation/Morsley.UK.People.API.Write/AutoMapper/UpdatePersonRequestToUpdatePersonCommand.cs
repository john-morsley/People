namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class UpdatePersonRequestToUpdatePersonCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public UpdatePersonRequestToUpdatePersonCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<UpdatePersonRequest, UpdatePersonCommand>();
    }
}
