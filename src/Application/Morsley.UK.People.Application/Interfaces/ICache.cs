namespace Morsley.UK.People.Application.Interfaces;

public interface ICache
{
    Task<string?> GetValueAsync(string key);

    Task SetValueAsync(string key, string value);
}