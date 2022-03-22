namespace Morsley.UK.People.API.Contracts.IoC;

public static class ModelsServiceCollectionExtensions
{
    public static IServiceCollection AddContracts(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.AddFluentValidation(configuration =>
        {
            configuration.RegisterValidatorsFromAssembly(executingAssembly);
        });

        services.AddTransient<IPropertyMappings, PropertyMappings>();

        return services;
    }
}
