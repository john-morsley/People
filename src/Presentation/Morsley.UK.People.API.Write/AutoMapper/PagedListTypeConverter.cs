namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class PagedListTypeConverter : ITypeConverter<PagedList<Person>, PagedList<PersonResponse>>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public PagedList<PersonResponse> Convert(
        PagedList<Person> source,
        PagedList<PersonResponse> destination,
        ResolutionContext context)
    {
        //if (source == null) return null;

        //var responses = new List<PersonResponse>();

        //foreach (var user in source)
        //{
        //    var response = context.Mapper.Map<PersonResponse>(user);
        //    responses.Add(response);
        //}
            
        //var currentPage = source.CurrentPage;
        //var totalPages = source.TotalPages;
        //var pageSize = source.PageSize; 
        //uint pageNumber = 1;
        //var totalCount = source.TotalCount;

        //var conversion = PagedList<PersonResponse>.Create(responses, pageNumber, pageSize, currentPage, totalPages);

        //return conversion;

        throw new NotImplementedException();
    }
}
