namespace Morsley.UK.People.API.Common.Swagger;

public static class ConfigureServices
{
    public static void ConfigureSwaggerWithoutAuthentication(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public static void ConfigureSwaggerWithAuthentication(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(_ =>
        {
            _.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    Description = "JWT authorization header using the bearer scheme",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http
                });
            _.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
                    },
                    new List<string>()
                }
            });
        });
    }

    //public static void AddAuthenticationAndAuthorization(
    //    this IServiceCollection services, 
    //    IConfiguration configuration)
    //{
    //    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(_ =>
    //    {
    //        _.TokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateActor = true,
    //            ValidateAudience = true,
    //            ValidateLifetime = true,
    //            ValidateIssuerSigningKey = true,
    //            ValidIssuer = configuration["Jwt:Issuer"],
    //            ValidAudience = configuration["Jwt:Audience"],
    //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    //        };
    //    });
    //    services.AddAuthorization(_ =>
    //    {
    //        _.FallbackPolicy = new AuthorizationPolicyBuilder()
    //            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    //            .RequireAuthenticatedUser()
    //            .Build();
    //    });
    //}
}