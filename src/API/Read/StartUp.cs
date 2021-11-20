namespace Users.API.Read;

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

        services.AddApplication();

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
