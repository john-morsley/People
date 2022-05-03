namespace Morsley.UK.People.Caching.Repositories;

public class Cache : ICache
{
    private readonly IRedisContext _context;

    public Cache(IRedisContext context)
    {
        _context = context;

    }

    //public void Dispose()
    //{
    //    _context = null;
    //}

    private IDatabase GetDatabase()
    {
        return _context.GetDatabase();
    }

    public async Task<string?> GetValueAsync(string key)
    {
        var database = GetDatabase();
        return await database.StringGetAsync(key);
    }

    public async Task SetValueAsync(string key, string value)
    {
        var database = GetDatabase();
        await database.StringSetAsync(key, value);
    }
}