namespace Users.API.Read.AutoMapper;

/// <summary>
/// 
/// </summary>
public class UserToUserResponse : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public UserToUserResponse()
    {
        CreateMap<Users.Domain.Models.User, Users.API.Models.Response.v1.UserResponse>();
    }
}
