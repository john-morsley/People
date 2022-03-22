namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class CreatePersonRequestToAddPersonCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public CreatePersonRequestToAddPersonCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<AddPersonRequest, AddPersonCommand>();
    }
}
