namespace Morsley.UK.People.Application.Interfaces;

public interface IFilter
{
    string Key { get; }

    string? Value { get; }
}