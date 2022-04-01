namespace Morsley.UK.People.Persistence.IoC;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, string persistenceKey)
    {
        var mongoContext = new MongoContext(configuration, persistenceKey);
        services.AddSingleton<IMongoContext>(mongoContext);
        //services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddSingleton<IPersonRepository, PersonRepository>();
        return services;
    }
}