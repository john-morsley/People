namespace Users.API.Read.AutoMapper;

public class UserToGetUserResponse : Profile
{
    public UserToGetUserResponse()
    {
        CreateMap<Users.Domain.Models.User, Users.API.Models.Response.v1.UserResponse>();
    }
}
