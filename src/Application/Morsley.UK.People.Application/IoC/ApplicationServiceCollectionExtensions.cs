namespace Morsley.UK.People.Application.IoC;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, bool logging = true, bool validation = true, bool caching = true)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(executingAssembly);

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.AddAutoMapper(assemblies);

        //if (logging) services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        //if (validation) services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        //if (caching) services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

        services.AddValidatorsFromAssembly(Assembly.Load("Morsley.UK.People.Application"));

        return services;
    }
}