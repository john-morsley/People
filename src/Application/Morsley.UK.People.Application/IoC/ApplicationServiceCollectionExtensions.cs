namespace Morsley.UK.People.Application.IoC;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(executingAssembly);

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.AddAutoMapper(assemblies);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

        services.AddValidatorsFromAssembly(Assembly.Load("Morsley.UK.People.Application"));

        return services;
    }
}