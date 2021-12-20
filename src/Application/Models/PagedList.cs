namespace Users.Application.Models;

public class PagedList<T> : List<T>, Users.Domain.Interfaces.IPagedList<T>
{
    protected PagedList(
        IEnumerable<T> items,
        int count,
        uint pageNumber,
        uint pageSize)
    {
        AddRange(items);
        TotalCount = (uint)count;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        if (count == 0)
        {
            TotalPages = 0;
        }
        else
        {
            TotalPages = (uint)((int)Math.Ceiling(count / (double)pageSize));
        }
    }

    public uint CurrentPage { get; private set; }

    public uint TotalPages { get; private set; }

    public uint PageSize { get; private set; }

    public uint TotalCount { get; private set; }

    public bool HasPrevious => CurrentPage > 1;

    public bool HasNext => CurrentPage < TotalPages;

    public static PagedList<T> Create(
        IQueryable<T> source,
        uint pageNumber,
        uint pageSize)
    {
        if (source == null) {throw new ArgumentNullException(nameof(source), "Cannot be null!");}
        if (pageNumber == 0) throw new ArgumentOutOfRangeException(nameof(pageNumber), "Must be greater than zero!");
        if (pageSize == 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "Must be greater than zero!");

        var count = source.Count();
        if (count == 0)
        {
            return new PagedList<T>(source, 0, 0, 0);
        }
        else
        {
            var items = source.Skip((int)((pageNumber - 1) * pageSize)).Take((int)pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
