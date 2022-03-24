namespace Morsley.UK.People.API.Write.Endpoints;

/// <summary>
/// 
/// </summary>
public static class SwaggerEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapSwaggerEndpoint(this WebApplication application)
    {
        application.MapGet("/", (HttpContext context) => context.Response.Redirect("/swagger/index.html"))
                   .AllowAnonymous()
                   .ExcludeFromDescription()
                   .WithName("GetSwagger");
    }
}