namespace Users.Domain.Interfaces;

public interface IGetOptions
{
    uint PageSize { get; }

    uint PageNumber { get; }

    string SearchQuery { get; }

    IEnumerable<IFilter> Filters { get; }

    IEnumerable<IOrdering> Orderings { get; }
}
