namespace Users.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class GetUserRequestToGetUserQuery : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public GetUserRequestToGetUserQuery()
    {
        CreateMap<Users.API.Models.Request.v1.GetUserRequest, Users.Application.Queries.GetUserQuery>();
    }
}
