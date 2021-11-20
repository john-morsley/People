namespace Users.API.Read.AutoMapper;

public class GetPageOfUsersRequestToGetPageOfUsersQuery : Profile
{
    public GetPageOfUsersRequestToGetPageOfUsersQuery()
    {
        CreateMap<Users.API.Models.Request.v1.GetPageOfUsersRequest, Users.Application.Queries.GetPageOfUsersQuery>();
    }
}
