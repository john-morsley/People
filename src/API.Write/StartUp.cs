using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Users.API.Shared.IoC;

namespace Users.API.Write
{
    public class StartUp
    {
        public IConfiguration Configuration { get; }
        
        public StartUp(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddVersioningAndSwagger(Configuration, Assembly.GetExecutingAssembly().GetName().Name);
            
            services.AddControllers();
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