namespace Users.API.Models.Request.v1;

public class GetPageOfUsersRequest
{
    private int _pageNumber = Defaults.DefaultPageNumber;
    private int _pageSize = Defaults.DefaultPageSize;

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
            if (_pageSize > Defaults.MaximumPageSize) _pageSize = Defaults.MaximumPageSize;
        }
    }

    /// <summary>
    /// The fields value is used to shape the returning data. i.e. fields=id,lastname
    /// </summary>
    public string? Fields { get; set; }

    /// <summary>
    /// The filter value is used for filtering results. i.e. filter=sex:male
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// The search value is used for searching within text searchable fields. i.e. search=aa
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// The sort value is used for sorting results. i.e. sort=LastName:asc,FirstName:asc
    /// </summary>
    public string Sort { get; set; } = Users.API.Models.Constants.Defaults.DefaultPageSort;
}
