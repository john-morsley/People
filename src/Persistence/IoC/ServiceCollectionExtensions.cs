using Microsoft.Extensions.DependencyInjection;
using Users.Domain.Interfaces;
using Users.Persistence.Repositories;

namespace Users.Persistence.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            return services;
        }
    }
}