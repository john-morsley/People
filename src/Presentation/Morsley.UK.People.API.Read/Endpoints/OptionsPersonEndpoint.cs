namespace Morsley.UK.People.API.Read.Endpoints;

/// <summary>
/// 
/// </summary>
public static class OptionsPersonEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapOptionsPersonEndpoint(this WebApplication application)
    {
        application.MapMethods("/api/person", new[] { "OPTIONS" }, (HttpResponse httpResponse) => OptionsPerson(httpResponse))
                   .AllowAnonymous()
                   .Produces(StatusCodes.Status200OK)
                   .WithName("OptionsPerson");
    }

    private static IResult OptionsPerson(HttpResponse httpResponse)
    {
        httpResponse.Headers.Add("Allow", "GET,HEAD,OPTIONS");

        return Results.Ok();
    }
}