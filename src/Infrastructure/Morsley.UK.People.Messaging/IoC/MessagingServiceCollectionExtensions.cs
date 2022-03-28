namespace Morsley.UK.People.Messaging.IoC;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddScoped<IEventBus, EventBus>();

        return services;
    }

}
