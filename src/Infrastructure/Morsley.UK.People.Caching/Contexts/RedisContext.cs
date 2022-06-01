using Morsley.UK.People.Caching.Configuration;

namespace Morsley.UK.People.Caching.Contexts;

public class RedisContext : IRedisContext
{
    private readonly IConfiguration _configuration;
    private readonly string _redisSettingsKey;
    private RedisSettings? _redisSettings;

    private ConnectionMultiplexer? _redis;

    public RedisContext(IConfiguration configuration, string redisSettingsKey)
    {
        _configuration = configuration;
        _redisSettingsKey = redisSettingsKey;
    }

    public IDatabase GetDatabase()
    {
        ConfigureCache();

        return _redis!.GetDatabase();
    }

    private void ConfigureCache()
    {
        if (_redis is not null) return;

        var section = _configuration.GetSection(_redisSettingsKey);
        _redisSettings = section.Get<RedisSettings>();
        
        var connectionString = GetConnectionString();
        _redis = ConnectionMultiplexer.Connect(connectionString);
    }

    public string GetConnectionString()
    {
        if (_redisSettings is null) throw new InvalidOperationException("RedisSettings cannot be null.");
        return GetConnectionString(_redisSettings);
    }

    private string GetConnectionString(RedisSettings redisSettings)
    {
        return $"{redisSettings.Host}:{redisSettings.Port}";
    }
}