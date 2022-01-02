namespace Users.API.Models.Request.v1;

public class GetPageOfUsersRequest
{
    const int MaxPageSize = 100;

    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>
    /// The page number of the page numbers available.
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
        set  
        { 
            _pageSize = value;
            if (_pageSize < 0) _pageSize = 1;
            if (_pageSize > MaxPageSize) _pageSize = MaxPageSize;
        }
    }

    /// <summary>
    /// The filter is used for filtering results. i.e. filter=sex:male
    /// </summary>
    public string Filter { get; set; }

    /// <summary>
    /// The search is used for searching within text searchable fields. i.e. search=abc
    /// </summary>
    public string Search { get; set; }

    /// <summary>
    /// The order by is used for sorting results. i.e. orderBy=FirstName:asc,LastName:asc
    /// </summary>
    public string OrderBy { get; set; } = "LastName";

    public string Fields { get; set; }
}
