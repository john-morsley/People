namespace Users.API.Write.AutoMapper;

public class PagedListTypeConverter : ITypeConverter<Users.Domain.Interfaces.IPagedList<Users.Domain.Models.User>,
                                                     Users.Domain.Interfaces.IPagedList<Users.API.Models.Response.v1.UserResponse>>
{
    public Users.Domain.Interfaces.IPagedList<Users.API.Models.Response.v1.UserResponse> Convert(
        Users.Domain.Interfaces.IPagedList<Users.Domain.Models.User> source,
        Users.Domain.Interfaces.IPagedList<Users.API.Models.Response.v1.UserResponse> destination,
        ResolutionContext context)
    {
        if (source == null) return null;

        var conversion = new Users.API.Models.PagedList<Users.API.Models.Response.v1.UserResponse>();

        foreach (var user in source)
        {
            var response = context.Mapper.Map<Users.API.Models.Response.v1.UserResponse>(user);
            conversion.Add(response);
        }
            
        conversion.CurrentPage = source.CurrentPage;
        conversion.TotalPages = source.TotalPages;
        conversion.PageSize = source.PageSize;
        conversion.TotalCount = source.TotalCount;

        return conversion;
    }
}
