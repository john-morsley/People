namespace Users.API.Read.AutoMapper;

public class UserToUserResponse : Profile
{
    public UserToUserResponse()
    {
        CreateMap<Users.Domain.Models.User, Users.API.Models.Response.v1.UserResponse>();
    }
}
