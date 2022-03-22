namespace Morsley.UK.People.API.Contracts.Responses;

public class PagedList<T> : IEnumerable<T>
{
    private readonly IEnumerable<T> _items;

    public PagedList(IEnumerable<T> items)
    {
        _items = items;
    }

    public uint CurrentPage { get; set; }

    public uint TotalPages { get; set; }

    public uint PageSize { get; set; }

    public uint TotalCount { get; set; }

    public bool HasPrevious { get; set; }

    public bool HasNext { get; set; }

    public int Count { get; set; }

    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
