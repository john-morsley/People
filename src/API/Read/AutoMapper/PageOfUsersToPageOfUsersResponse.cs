namespace Users.API.Read.AutoMapper;

public class PageOfUsersToPageOfUsersResponse : Profile
{
    public PageOfUsersToPageOfUsersResponse()
    {
        CreateMap<IPagedList<Users.Domain.Models.User>, Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>().ConvertUsing<PagedListTypeConverter>(); ;
    }
}
