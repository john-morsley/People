namespace Morsley.UK.People.Synchronizer;

public class NoCache : ICache
{
    public Task DeleteValueAsync(string key)
    {
        return Task.CompletedTask;
    }

    public Task<string?> GetValueAsync(string key)
    {
        return (Task<string?>)Task.CompletedTask;
    }

    public Task SetValueAsync(string key, string value)
    {
        return Task.CompletedTask;
    }
}
