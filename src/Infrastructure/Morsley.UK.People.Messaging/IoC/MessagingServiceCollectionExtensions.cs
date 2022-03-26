namespace Morsley.UK.People.Messaging.IoC;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        //var mongoContext = new MongoContext(configuration);
        // ToDo --> Is this Mongo Context valid?
        //services.AddSingleton<IMongoContext>(mongoContext);
        //services.AddScoped<IPersonRepository, PersonRepository>();

        services.AddTransient<IEventBus, EventBus>(); //sp =>
        //{
            //var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            //return new RabbitMqBus(sp.GetService<IMediator>(), scopeFactory, configuration.GetSection("MessageBroker:Host").Value, sp.GetService<ILogger<RabbitMqBus>>());
        //});

        return services;
    }

}
