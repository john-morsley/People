namespace Users.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class PersistencePagedListToApiPagedList : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public PersistencePagedListToApiPagedList()
    {
        CreateMap<Users.Domain.Models.User,
                    Users.API.Models.Response.v1.UserResponse>();

        CreateMap<Users.Domain.Interfaces.IPagedList<Users.Domain.Models.User>,
                  Users.Domain.Interfaces.IPagedList<Users.API.Models.Response.v1.UserResponse>>()
            .ConvertUsing<PagedListTypeConverter>();
    }
}
