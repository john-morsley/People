namespace Users.API.Models.IoC;

public static class ModelsServiceCollectionExtensions
{
    public static IServiceCollection AddModels(this IServiceCollection services)
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
