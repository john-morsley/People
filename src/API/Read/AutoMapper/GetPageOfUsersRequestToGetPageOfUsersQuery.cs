namespace Users.API.Read.AutoMapper;

/// <summary>
/// 
/// </summary>
public class GetPageOfUsersRequestToGetPageOfUsersQuery : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public GetPageOfUsersRequestToGetPageOfUsersQuery()
    {
        CreateMap<Users.API.Models.Request.v1.GetPageOfUsersRequest, Users.Application.Queries.GetPageOfUsersQuery>();
    }
}
