using Users.Persistence.Contexts;
using Users.Persistence.Repositories;

namespace Users.Persistence.IoC;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoContext = new MongoContext(configuration);
        services.AddSingleton<IMongoContext>(mongoContext);
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
