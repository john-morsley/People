using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Users.API.Shared.IoC;
using Users.Persistence.IoC;

namespace Users.API.Read
{
    public class StartUp
    {
        public IConfiguration Configuration { get; }

        public StartUp(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddVersioningAndSwagger(Configuration, Assembly.GetExecutingAssembly().GetName().Name);

            services.AddControllers(configure =>
            {
                configure.ReturnHttpNotAcceptable = true;
            });

            //var settings = Shared.Environment.GetEnvironmentVariableValueByKey(Shared.Constants.EnvironmentVariables.UsersPersistenceKey);

            //if (string.IsNullOrEmpty(settings))
            //{
            //    Log.Fatal("Could not determine Persistence Key! :-(");
            //    return;
            //}

            //services.AddPersistence(settings);
            services.AddPersistence();
        }


        public void Configure(
            IApplicationBuilder applicationBuilder,
            IWebHostEnvironment webHostEnvironment,
            IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            if (webHostEnvironment.IsDevelopment()) applicationBuilder.UseDeveloperExceptionPage();

            applicationBuilder.ConfigureVersioningAndSwagger(apiVersionDescriptionProvider);
            
            applicationBuilder.UseHttpsRedirection();

            applicationBuilder.UseRouting();

            applicationBuilder.UseAuthorization();

            applicationBuilder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}