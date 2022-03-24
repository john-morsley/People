namespace Morsley.UK.People.API.Read.Endpoints;

/// <summary>
/// 
/// </summary>
public static class HeadPersonEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapHeadPersonEndpoint(this WebApplication application)
    {
        application.MapMethods("/api/person/{id}", new[] { "HEAD" }, async(
                    [FromRoute] Guid id,
                    [FromQuery] string fields,
                    HttpResponse httpResponse,
                    IMapper mapper,
                    IMediator mediator,
                    ILogger logger)
                    =>
                    await HeadPerson(new GetPersonRequest { Id = id, Fields = fields }, httpResponse, mapper, mediator, logger))
                   //.Accepts<GetPersonRequest>("application/json")
                   .Produces(StatusCodes.Status200OK)
                   .WithName("HeadPerson");
    }

    //[HttpHead]
    private async static Task<IResult> HeadPerson(
        GetPersonRequest request,
        HttpResponse httpResponse,
        IMapper mapper,
        IMediator mediator,
        ILogger logger)
    {
        if (request.Id == Guid.Empty) return Results.BadRequest();

        var personResponse = await PersonService.TryGetPerson(request, mapper, mediator, logger);

        if (personResponse == null) return Results.NoContent();

        var shapedPerson = personResponse.ShapeData(request.Fields);
        var linkedShapedPerson = LinksHelper.AddLinks(shapedPerson!, personResponse.Id);

        httpResponse.ContentLength = ContentHelper.CalculateLength(linkedShapedPerson!);

        return Results.Ok();
    }
}