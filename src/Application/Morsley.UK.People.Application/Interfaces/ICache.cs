namespace Morsley.UK.People.Application.Interfaces;

public interface ICache
{
    Task DeleteValueAsync(string key);

    Task<string?> GetValueAsync(string key);

    Task SetValueAsync(string key, string value);
}