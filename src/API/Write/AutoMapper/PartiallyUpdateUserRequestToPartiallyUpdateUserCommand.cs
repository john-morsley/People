namespace Users.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class PartiallyUpdateUserRequestToPartiallyUpdateUserCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public PartiallyUpdateUserRequestToPartiallyUpdateUserCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.API.Models.Request.v1.PartiallyUpdateUserRequest, Users.Application.Commands.PartiallyUpdateUserCommand>();
    }
}
