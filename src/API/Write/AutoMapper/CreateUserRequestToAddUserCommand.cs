namespace Users.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class CreateUserRequestToAddUserCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public CreateUserRequestToAddUserCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>()
        //    .ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>()
        //    .ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.API.Models.Request.v1.AddUserRequest, Users.Application.Commands.AddUserCommand>();
    }
}
