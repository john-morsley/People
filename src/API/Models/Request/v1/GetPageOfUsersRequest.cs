namespace Users.API.Models.Request.v1;

public class GetPageOfUsersRequest
{
    const int MaxPageSize = 100;

    private int _pageNumber = 1;
    private int _pageSize = 10;

    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            _pageNumber = value;
            if (_pageNumber < 0) _pageNumber = 1;
        }
    }

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

    public string Filter { get; set; }

    public string SearchQuery { get; set; }
}
