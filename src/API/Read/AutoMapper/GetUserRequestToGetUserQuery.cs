namespace Users.API.Read.AutoMapper;

public class GetUserRequestToGetUserQuery : Profile
{
    public GetUserRequestToGetUserQuery()
    {
        CreateMap<Users.API.Models.Request.v1.GetUserRequest, Users.Application.Queries.GetUserQuery>();
    }
}
