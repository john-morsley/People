namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// The AutoMapper profile for converting an AddPersonRequest to AddPersonCommand
/// </summary>
public class AddPersonRequestToAddPersonCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public AddPersonRequestToAddPersonCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>().ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>().ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<AddPersonRequest, AddPersonCommand>();
    }
}
