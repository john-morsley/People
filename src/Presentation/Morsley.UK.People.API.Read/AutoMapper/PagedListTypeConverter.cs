namespace Morsley.UK.People.API.Read.AutoMapper;

/// <summary>
/// 
/// </summary>
public class PagedListTypeConverter : ITypeConverter<Morsley.UK.People.Application.Models.PagedList<Person>, Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse>>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse> Convert(Morsley.UK.People.Application.Models.PagedList<Person> source,
        Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse> destination,
        ResolutionContext context)
    {
        var people = new List<PersonResponse>();

        foreach (var person in source)
        {
            var response = context.Mapper.Map<PersonResponse>(person);
            people.Add(response);
        }

        var conversion = new Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse>(people);

        // ToDo --> This should probably be done with recursion
        // See PersonResponseExtensions

        conversion.Count = source.Count;
        conversion.CurrentPage = source.CurrentPage;
        conversion.TotalPages = source.TotalPages;
        conversion.PageSize = source.PageSize;
        conversion.TotalCount = source.TotalCount;
        conversion.HasPrevious = source.HasPrevious;
        conversion.HasNext = source.HasNext;

        return conversion;
    }
}