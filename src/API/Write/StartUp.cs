namespace Users.API.Write;

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
