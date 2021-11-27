
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
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new Users.API.Models.Shared.PagedListJsonConverter());
        });

        //var settings = Shared.Environment.GetEnvironmentVariableValueByKey(Shared.Constants.EnvironmentVariables.UsersPersistenceKey);

        //if (string.IsNullOrEmpty(settings))
        //{
        //    Log.Fatal("Could not determine Persistence Key! :-(");
        //    return;
        //}

        services.AddPersistence(Configuration);
        services.AddApplication();

        

        //services.AddPersistence(settings);        
    }


    public void Configure(
        IApplicationBuilder applicationBuilder,
        IWebHostEnvironment webHostEnvironment,
        IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        if (webHostEnvironment.IsDevelopment())
        {
            applicationBuilder.UseDeveloperExceptionPage();
        }
        else
        {
            //applicationBuilder.UseExceptionHandler(applicationBuilder => {
            //    applicationBuilder.Run(async context => {
            //        context.Response.StatusCode = 500;
            //        await context.Response.WriteAsync("Oops, I didn't expect that to happen!");
            //    });
            //});

            applicationBuilder.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    //var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; ;                        
                    context.Response.ContentType = "text/html";
                    context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Oops, I didn't expect that to happen! :-(";
                    // ToDo --> Log error
                });
            });
        }

        applicationBuilder.ConfigureVersioningAndSwagger(apiVersionDescriptionProvider);
            
        applicationBuilder.UseHttpsRedirection();

        applicationBuilder.UseRouting();

        applicationBuilder.UseAuthorization();

        applicationBuilder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
