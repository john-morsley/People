namespace Users.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class DeleteUserRequestToDeleteUserCommand : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public DeleteUserRequestToDeleteUserCommand()
    {
        CreateMap<Users.API.Models.Request.v1.DeleteUserRequest, Users.Application.Commands.DeleteUserCommand>();
    }
}
