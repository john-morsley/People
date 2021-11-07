using Users.Domain.Enumerations;

namespace Users.Domain.Interfaces
{
    public interface IOrdering
    {
        string Key { get; }

        SortOrder Order { get; }
    }
}