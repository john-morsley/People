namespace Morsley.UK.People.Application.Models;

public class Ordering : IOrdering
{
    public Ordering(string key, SortOrder order)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key cannot be null or empty!", nameof(key));

        Key = key;
        Order = order;
    }

    public string Key { get; }

    public SortOrder Order { get; }
}