namespace Morsley.UK.People.API.Write.Endpoints;

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
        application.MapMethods("/api/person", new[] { "OPTIONS" }, (HttpResponse httpResponse) => OptionsPeople(httpResponse))
                   .Produces(StatusCodes.Status200OK)
                   .WithName("OptionsPerson")
                   .AllowAnonymous();
    }

    private static IResult OptionsPeople(HttpResponse httpResponse)
    {
        httpResponse.Headers.Add("Allow", "DELETE,PATCH,POST,PUT");

        return Results.Ok();
    }
}