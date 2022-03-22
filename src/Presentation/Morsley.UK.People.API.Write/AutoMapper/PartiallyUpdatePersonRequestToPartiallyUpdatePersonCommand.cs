namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class PartiallyUpdatePersonRequestToPartiallyUpdatePersonCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public PartiallyUpdatePersonRequestToPartiallyUpdatePersonCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<PartiallyUpdatePersonRequest, PartiallyUpdatePersonCommand>();
    }
}
