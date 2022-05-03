namespace Morsley.UK.People.API.Contracts.Responses;

public class PagedList<T> : IList<T>
{
    public readonly IEnumerable<T> Items;

    public PagedList()
    {
        Items = new List<T>();
    }

    public PagedList(IEnumerable<T> items)
    {
        Items = items;
    }

    public uint CurrentPage { get; set; }

    public uint TotalPages { get; set; }

    public uint PageSize { get; set; }

    public uint TotalCount { get; set; }

    public bool HasPrevious { get; set; }

    public bool HasNext { get; set; }

    public void Add(T item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(T item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    public int Count { get; set; }
    public bool IsReadOnly { get; }

    public IEnumerator<T> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(T item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public T this[int index]
    {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
    }
}
