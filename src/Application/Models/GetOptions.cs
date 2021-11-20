namespace Users.Application.Models;

public class GetOptions : Users.Domain.Interfaces.IGetOptions
{
    private readonly IList<Users.Domain.Interfaces.IFilter> _filters;
    private readonly IList<Users.Domain.Interfaces.IOrdering> _orderings;

    public GetOptions()
    {
        _filters = new List<Users.Domain.Interfaces.IFilter>();
        _orderings = new List<Users.Domain.Interfaces.IOrdering>();
    }

    public int PageSize { get; set; } = 25;

    public int PageNumber { get; set; } = 1;

    public string? SearchQuery { get; set; }

    public IEnumerable<Users.Domain.Interfaces.IFilter> Filters => _filters;

    public IEnumerable<Users.Domain.Interfaces.IOrdering> Orderings => _orderings;

    public void AddFilter(Users.Domain.Interfaces.IFilter filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));

        if (!CanAddFilter(filter)) throw new ArgumentException("Duplicate!", nameof(filter));
        _filters.Add(filter);
    }

    public void AddOrdering(Users.Domain.Interfaces.IOrdering ordering)
    {
        if (ordering == null) throw new ArgumentNullException(nameof(ordering));

        if (!CanAddOrdering(ordering)) throw new ArgumentException("Duplicate!", nameof(ordering));
        _orderings.Add(ordering);
    }

    private bool CanAddFilter(Users.Domain.Interfaces.IFilter filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));

        if (_filters.Contains(filter)) return false;

        return true;
    }

    private bool CanAddOrdering(Users.Domain.Interfaces.IOrdering ordering)
    {
        if (ordering == null) throw new ArgumentNullException(nameof(ordering));

        if (_orderings.Contains(ordering)) return false;

        return true;
    }
}
