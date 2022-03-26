namespace Morsley.UK.People.Application.Interfaces;

public interface IPagedList<T> : IList<T>
{
    uint CurrentPage { get; }

    uint TotalPages { get; }

    uint PageSize { get; }

    /// <summary>
    /// The total number of T. 
    /// i.e. If we have 10 complete pages with a page size of 10 TotalCount will be 100.
    /// </summary>
    uint TotalCount { get; }

    bool HasPrevious { get; }

    bool HasNext { get; }
}