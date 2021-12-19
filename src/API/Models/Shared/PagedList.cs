using System.Collections;

namespace Users.API.Models.Shared;


public class PagedList<T> : Domain.Interfaces.IPagedList<T>
{
    public T this[int index] 
    { 
        get => _items[index];
        set => _items[index] = value; 
    }

    protected List<T> _items;

    public PagedList()
    {
        _items = new List<T>();
    }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public bool HasPrevious => CurrentPage > 1;

    public bool HasNext => CurrentPage < TotalPages;

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        _items.Add(item);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool Contains(T item)
    {
        return _items.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return _items.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        _items.Insert(index, item);
    }

    public bool Remove(T item)
    {
        return _items.Remove(item);
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
