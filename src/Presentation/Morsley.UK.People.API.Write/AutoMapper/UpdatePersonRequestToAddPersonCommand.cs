namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class UpdatePersonRequestToAddPersonCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public UpdatePersonRequestToAddPersonCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<UpdatePersonRequest, AddPersonCommand>();
    }
}
