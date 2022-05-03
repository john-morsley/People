namespace Morsley.UK.People.Caching.IoC;

public static class CachingServiceCollectionExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration, string cachingKey)
    {
        var redisContext = new RedisContext(configuration, cachingKey);

        services.AddSingleton<IRedisContext>(redisContext);

        //services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
        
        services.AddSingleton<ICache, Cache>();
        
        return services;
    }
}