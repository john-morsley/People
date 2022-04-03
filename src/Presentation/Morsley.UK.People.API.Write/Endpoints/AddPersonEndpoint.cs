namespace Morsley.UK.People.API.Write.Endpoints;

/// <summary>
/// 
/// </summary>
public static class AddPersonEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapAddPersonEndpoint(this WebApplication application)
    {
        application.MapMethods("/api/person", new [] { "POST" }, async (
                    [FromBody] AddPersonRequest request,
                    [FromServices] IValidator<AddPersonRequest> validator,
                    [FromServices] IMapper mapper,
                    [FromServices] IMediator mediator,
                    [FromServices] ILogger logger)
                    =>
                    await AddPerson(request, validator, mapper, mediator, logger))
                   .Accepts<AddPersonRequest>("application/json")
                   .Produces<PersonResponse>(StatusCodes.Status200OK, "application/json")
                   .Produces(StatusCodes.Status422UnprocessableEntity)
                   .Produces(StatusCodes.Status500InternalServerError)
                   .WithName("AddPerson").WithDisplayName("Add Person");
    }

    [HttpPost]
    private async static Task<IResult> AddPerson(
        AddPersonRequest request,
        IValidator<AddPersonRequest> validator,
        IMapper mapper, 
        IMediator mediator, 
        ILogger logger)
    {
        logger.Debug("AddPerson");

        if (!ValidatorHelper.IsRequestValid(request, validator, out var problemDetails)) return Results.UnprocessableEntity(problemDetails);

        var personResponse = await TryAddPerson(request, mapper, mediator, logger);

        if (personResponse == null) return Results.UnprocessableEntity();

        var shapedPersonWithLinks = PersonResponseHelper.ShapePersonWithLinks(personResponse);

        return Results.Created($"http://localhost/api/person/{personResponse.Id}", shapedPersonWithLinks);
    }

    private async static Task<PersonResponse?> TryAddPerson(
        AddPersonRequest request, 
        IMapper mapper, 
        IMediator mediator, 
        ILogger logger)
    {
        try
        {
            var person = await PersonHelper.AddPerson(request, mapper, mediator, logger);
            var response = PersonResponseHelper.From(person, mapper);
            return response;
        }
        catch (Exception e)
        {
            logger.Error(e, "Error occurred whilst trying to add a person!");
            throw;
        }
    }
}