namespace Morsley.UK.People.API.Read.Endpoints;

/// <summary>
/// 
/// </summary>
public static class HeadPeopleEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapHeadPeopleEndpoint(this WebApplication application)
    {
        application.MapMethods("/api/people", new [] { "HEAD" }, async (
                    GetPeopleRequest request,
                    HttpResponse httpResponse,
                    IMapper mapper,
                    IMediator mediator,
                    ILogger logger)
                    => 
                    await HeadPeople(request, httpResponse,mapper, mediator, logger))
                   .Accepts<GetPeopleRequest>("application/json")
                   .Produces<IPagedList<PersonResponse>>(StatusCodes.Status200OK, "application/json")
                   .WithName("HeadPeople");
    }

    private async static Task<IResult> HeadPeople(
        GetPeopleRequest request,
        HttpResponse httpResponse,
        IMapper mapper,
        IMediator mediator,
        ILogger logger)
    {
        var pageOfPersonResponses = await PeopleService.TryGetPeople(request, mapper, mediator, logger);

        if (pageOfPersonResponses.Count == 0) return Results.NoContent();

        // Shape Persons...
        var shapedPageOfPersons = pageOfPersonResponses.ShapeData(request.Fields);

        // Add Metadata links...         
        var shapedPageOfPersonsWithLinks = LinksHelper.AddLinks(shapedPageOfPersons);
        var pageOfPersonsLinks = LinksHelper.CreateLinksForPageOfPersons(request, pageOfPersonResponses);

        var shapedPageOfPersonsWithLinksWithSelfLink = new ExpandoObject() as IDictionary<string, object>;
        shapedPageOfPersonsWithLinksWithSelfLink.Add("_embedded", shapedPageOfPersonsWithLinks);
        shapedPageOfPersonsWithLinksWithSelfLink.Add("_links", pageOfPersonsLinks);

        httpResponse.ContentLength = ContentHelper.CalculateLength(shapedPageOfPersonsWithLinksWithSelfLink);

        return Results.Ok();
    }
}