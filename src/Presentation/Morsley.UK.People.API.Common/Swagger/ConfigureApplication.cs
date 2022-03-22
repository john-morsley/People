namespace Morsley.UK.People.API.Common.Swagger;

public static class ConfigureApplication
{
    public static void ConfigureSwagger(this WebApplication application)
    {
        application.UseSwagger();
        application.UseSwaggerUI();
    }
}