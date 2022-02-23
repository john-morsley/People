namespace Users.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class AddUserRequestToAddUserCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public AddUserRequestToAddUserCommand()
    {
        //CreateMap<string?, Domain.Enumerations.Sex?>().ConvertUsing(new StringSexToEnumSexConverter());

        //CreateMap<string?, Domain.Enumerations.Title?>().ConvertUsing(new StringTitleToEnumTitleConverter());

        CreateMap<Users.API.Models.Request.v1.AddUserRequest, Application.Commands.AddUserCommand>();
    }
}
