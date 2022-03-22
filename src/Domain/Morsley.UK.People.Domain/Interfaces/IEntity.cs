namespace Morsley.UK.People.Domain.Interfaces
{
    public interface IEntity<T> : IEquatable<T>
    {
        T Id { get; }
    }
}
