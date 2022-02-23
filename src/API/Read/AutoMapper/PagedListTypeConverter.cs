namespace Users.API.Read.AutoMapper;

/// <summary>
/// 
/// </summary>
public class PagedListTypeConverter : ITypeConverter<Users.Domain.Interfaces.IPagedList<Users.Domain.Models.User>,
                                                     Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public Users.API.Models.Shared.PagedList<API.Models.Response.v1.UserResponse> Convert(
        IPagedList<Domain.Models.User> source,
        Users.API.Models.Shared.PagedList<API.Models.Response.v1.UserResponse> destination,
        ResolutionContext context)
    {
        var conversion = new Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>();

        foreach (var user in source)
        {
            var response = context.Mapper.Map<API.Models.Response.v1.UserResponse>(user);
            conversion.Add(response);
        }

        conversion.CurrentPage = source.CurrentPage;
        conversion.TotalPages = source.TotalPages;
        conversion.PageSize = source.PageSize;
        conversion.TotalCount = source.TotalCount;

        return conversion;
    }
}