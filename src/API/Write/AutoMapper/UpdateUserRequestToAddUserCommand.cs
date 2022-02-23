namespace Users.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class UpdateUserRequestToAddUserCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public UpdateUserRequestToAddUserCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.API.Models.Request.v1.UpdateUserRequest,
                  Users.Application.Commands.AddUserCommand>();
    }
}
