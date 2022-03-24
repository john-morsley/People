namespace Morsley.UK.People.API.Contracts.Requests;

/// <summary>
/// The object used to request people.
/// </summary>
public record GetPeopleRequest
{
    private int _pageNumber = Defaults.DefaultPageNumber;
    private int _pageSize = Defaults.DefaultPageSize;

    public GetPeopleRequest()
    {
        
    }

    public GetPeopleRequest(
        int? pageNumber, 
        int? pageSize, 
        string? fields, 
        string? filter, 
        string? search, 
        string? sort)
    {
        if (pageNumber is not null) PageNumber = pageNumber.Value;
        if (pageSize is not null) PageSize = pageSize.Value;
        Fields = fields;
        Filter = filter;
        Search = search;
        if (sort is not null) Sort = sort;
    }

    /// <summary>
    /// The page number to be requested.
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            _pageNumber = value;
            if (_pageNumber < 0) _pageNumber = 1;
        }
    }

    /// <summary>
    /// The size of the page that is to be returned.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set {
            _pageSize = value;
            if (_pageSize < 0) _pageSize = 1;
            if (_pageSize > Defaults.MaximumPageSize) _pageSize = Defaults.MaximumPageSize;
        }
    }

    /// <summary>
    /// The fields value is used to shape the returning data. i.e. fields=id,lastname
    /// </summary>
    public string? Fields { get; set; }

    /// <summary>
    /// The filter value is used for filtering results. i.e. filter=sex:male or filter=sex (no value)
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// The search value is used for searching within text searchable fields. i.e. search=abc
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// The sort value is used for sorting results. i.e. sort=LastName:asc,FirstName:asc
    /// </summary>
    public string Sort { get; set; } = Defaults.DefaultPageSort;

    public static ValueTask<GetPeopleRequest?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        int.TryParse(httpContext.Request.Query["pagenumber"], out var pageNumber);
        int.TryParse(httpContext.Request.Query["pagesize"], out var pageSize);
        var fields = httpContext.Request.Query["fields"];
        var filter = httpContext.Request.Query["filter"];
        var search = httpContext.Request.Query["search"];
        var sort = httpContext.Request.Query["sort"];

        var request = new GetPeopleRequest
        {
            PageNumber = pageNumber > 0 ? pageNumber : Defaults.DefaultPageNumber,
            PageSize = pageSize > 0 ? pageSize : Defaults.DefaultPageSize,
            Fields = fields.Count == 1 ? fields[0] : null,
            Filter = filter.Count == 1 ? filter[0] : null,
            Search = search.Count == 1 ? search[0] : null,
            Sort = sort.Count == 1 ? sort[0] : Defaults.DefaultPageSort
        };

        return ValueTask.FromResult<GetPeopleRequest?>(request);
    }

    public static bool TryParse(string value, out GetPeopleRequest request)
    {
        request = new GetPeopleRequest();
        return true;
    }

}