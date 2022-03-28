namespace Morsley.UK.People.Persistence.IoC;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoContext = new MongoContext(configuration);
        services.AddSingleton<IMongoContext>(mongoContext);
        services.AddScoped<IPersonRepository, PersonRepository>();
        return services;
    }
}