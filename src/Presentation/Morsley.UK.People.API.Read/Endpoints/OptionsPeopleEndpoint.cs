namespace Morsley.UK.People.API.Read.Endpoints;

/// <summary>
/// 
/// </summary>
public static class OptionsPeopleEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapOptionsPeopleEndpoint(this WebApplication application)
    {
        application.MapMethods("/api/people", new[] { "OPTIONS" }, (HttpResponse httpResponse) => OptionsPeople(httpResponse))
                   .AllowAnonymous()
                   .Produces(StatusCodes.Status200OK)
                   .WithName("OptionsPeople");
    }

    private static IResult OptionsPeople(HttpResponse httpResponse)
    {
        httpResponse.Headers.Add("Allow", "GET,HEAD,OPTIONS");

        return Results.Ok();
    }
}