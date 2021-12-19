namespace Users.API.Models.Shared;

public class Pagination
{
    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public string PreviousPageLink { get; set; }

    public string NextPageLink { get; set; }
}
