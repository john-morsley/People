namespace Users.Domain.Interfaces;

public interface IPagedList<T> : IList<T>
{
    int CurrentPage { get; }

    int TotalPages { get; }

    int PageSize { get; }

    /// <summary>
    /// The total number of T. 
    /// i.e. If we have 10 complete pages with a page size of 10 TotalCount will be 100.
    /// </summary>
    int TotalCount { get; }

    bool HasPrevious { get; }

    bool HasNext { get; }
}
