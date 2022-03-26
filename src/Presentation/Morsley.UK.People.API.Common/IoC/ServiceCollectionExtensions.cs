//using Morsley.UK.People.API.Common.VersioningAndSwagger;

using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Morsley.UK.People.API.Common.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVersioningAndSwagger(
        this IServiceCollection services,
        IConfiguration configuration,
        string callingAssemblyName)
    {
        services.Configure<OpenApiInfo>(configuration.GetSection(nameof(OpenApiInfo)));
        
        //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        var methodsOrder = new string[] { "OPTIONS", "HEAD", "GET", "POST", "PUT", "PATCH", "DELETE", "TRACE" };

        services.AddSwaggerGen(options =>
        {
            //options.OperationFilter<SwaggerDefaultValues>();
            options.IncludeXmlComments(GetXmlCommentsFilePath(callingAssemblyName));
            options.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{Array.IndexOf(methodsOrder, apiDesc?.HttpMethod?.ToUpper())}");
        });

        services.AddSwaggerGenNewtonsoftSupport();

        //services.AddApiVersioning(options =>
        //{
        //    options.AssumeDefaultVersionWhenUnspecified = true;
        //    options.DefaultApiVersion = ApiVersion.Default;
        //    options.ReportApiVersions = true;
        //});
        
        //services.AddVersionedApiExplorer(options =>
        //{
        //    options.GroupNameFormat = "'v'VVV";
        //    options.SubstituteApiVersionInUrl = true;
        //});
            
        return services;
    }

    private static string GetXmlCommentsFilePath(string callingAssemblyName)
    {
        var basePath = PlatformServices.Default.Application.ApplicationBasePath;
        var fileName = $"{callingAssemblyName}.xml";
        return Path.Combine(basePath, fileName);
    }

    public static void AddAuthenticationAndAuthorization(
        this IServiceCollection services,
        IConfiguration configuration,
        Serilog.ILogger logger)
    {
        logger.Information("AddAuthenticationAndAuthorization");
        logger.Information("AddAuthenticationAndAuthorization - Jwt:Issuer --> {0}", configuration["Jwt:Issuer"]);
        logger.Information("AddAuthenticationAndAuthorization - Jwt:Audience --> {0}", configuration["Jwt:Audience"]);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(_ =>
        {
            _.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            };
        });
        services.AddAuthorization(_ =>
        {
            _.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

    }

}