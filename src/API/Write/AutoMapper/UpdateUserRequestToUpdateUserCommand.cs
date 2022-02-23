namespace Users.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class UpdateUserRequestToUpdateUserCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public UpdateUserRequestToUpdateUserCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.API.Models.Request.v1.UpdateUserRequest,
                  Users.Application.Commands.UpdateUserCommand>();
    }
}
