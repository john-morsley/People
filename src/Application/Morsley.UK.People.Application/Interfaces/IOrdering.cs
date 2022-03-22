namespace Morsley.UK.People.Application.Interfaces
{
    public interface IOrdering
    {
        string Key { get; }

        SortOrder Order { get; }
    }
}
