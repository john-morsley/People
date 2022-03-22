namespace Morsley.UK.People.Services.IoC;

public static class ServicesServiceCollectionExtensions
{
    public static IServiceCollection AddReadServices(this IServiceCollection services)
    {
        services.AddScoped<IReadService, ReadService>();
        return services;
    }

    public static IServiceCollection AddWriteServices(this IServiceCollection services)
    {
        services.AddScoped<IWriteService, WriteService>();
        return services;
    }
}