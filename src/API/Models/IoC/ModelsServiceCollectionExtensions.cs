using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Users.API.Models.IoC;

public static class ModelsServiceCollectionExtensions
{
    public static IServiceCollection AddModelValidation(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        //services.AddMediatR(executingAssembly);

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        //services.AddAutoMapper(assemblies);

        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

        services.AddFluentValidation(configuration =>
        {
            //configuration.RegisterValidatorsFromAssembly();
            configuration.RegisterValidatorsFromAssembly(executingAssembly);
        });


        return services;
    }
}
