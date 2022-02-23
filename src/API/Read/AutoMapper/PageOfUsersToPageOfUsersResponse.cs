namespace Users.API.Read.AutoMapper;

/// <summary>
/// 
/// </summary>
public class PageOfUsersToPageOfUsersResponse : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public PageOfUsersToPageOfUsersResponse()
    {
        CreateMap<DateTime?, string?>().ConvertUsing<NullableDateTimeTypeConverter>();
        CreateMap<Users.Domain.Interfaces.IPagedList<Users.Domain.Models.User>, Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>().ConvertUsing<PagedListTypeConverter>();
    }
}
