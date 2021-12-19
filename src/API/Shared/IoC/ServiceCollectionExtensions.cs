namespace Users.API.Shared.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVersioningAndSwagger(
        this IServiceCollection services,
        IConfiguration configuration,
        string callingAssemblyName)
    {
        services.Configure<OpenApiInfo>(configuration.GetSection(nameof(OpenApiInfo)));
        
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        var methodsOrder = new string[] { "OPTIONS", "HEAD", "GET", "POST", "PUT", "PATCH", "DELETE", "TRACE" };

        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<SwaggerDefaultValues>();
            options.IncludeXmlComments(GetXmlCommentsFilePath(callingAssemblyName));
            options.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{Array.IndexOf(methodsOrder, apiDesc.HttpMethod.ToUpper())}");
        });

        services.AddSwaggerGenNewtonsoftSupport();

        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = ApiVersion.Default;
            options.ReportApiVersions = true;
        });
        
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
            
        return services;
    }

    private static string GetXmlCommentsFilePath(string callingAssemblyName)
    {
        var basePath = PlatformServices.Default.Application.ApplicationBasePath;
        var fileName = $"{callingAssemblyName}.xml";
        return Path.Combine(basePath, fileName);
    }
}
