namespace Morsley.UK.People.API.Write.Endpoints;

/// <summary>
/// 
/// </summary>
public static class UpdatePersonEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapUpdatePersonEndpoint(this WebApplication application)
    {
        application.MapMethods("/api/person/{id}", new[] { "PUT" }, async (
                    UpdatePersonRequest request,
                    [FromServices] IValidator<UpdatePersonRequest> validator,
                    [FromServices] IMapper mapper,
                    [FromServices] IMediator mediator,
                    [FromServices] ILogger logger,
                    [FromServices] ActivitySource source)
                    =>
                    await UpsertPerson(request, validator, mapper, mediator, logger, source))
                    .Accepts<UpdatePersonRequest>("application/json")
                    .Produces<PersonResponse>(StatusCodes.Status200OK, "application/json")
                    .Produces(StatusCodes.Status422UnprocessableEntity)
                    .Produces(StatusCodes.Status500InternalServerError)
                    .WithName("UpdatePerson");
    }

    private async static Task<IResult> UpsertPerson(
        UpdatePersonRequest request,
        IValidator<UpdatePersonRequest> validator,
        IMapper mapper,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        if (!ValidatorHelper.IsRequestValid(request, validator, out var problemDetails)) return Results.UnprocessableEntity(problemDetails);

        var personId = request.Id;

        // As we are upserting, we either update or add...
        if (await PersonHelper.DoesPersonExist(personId, mediator, logger))
        {
            // Update existing person...
            return await UpdatePerson(request, mapper, mediator, logger, source);
        }

        // Add new person...
        return await AddPerson(request, mapper, mediator, logger, source);
    }

    private async static Task<IResult> AddPerson(
        UpdatePersonRequest request, 
        IMapper mapper, 
        IMediator mediator, 
        ILogger logger,
        ActivitySource source)
    {
        var personResponse = await TryAddPerson(request, mapper, mediator, logger, source);
        var shapedPersonResponseWithLinks = PersonResponseHelper.ShapePersonWithLinks(personResponse);
        return Results.Created($"https://localhost/api/person/{personResponse.Id}", shapedPersonResponseWithLinks);
    }

    private async static Task<IResult> UpdatePerson(
        UpdatePersonRequest request, 
        IMapper mapper, 
        IMediator mediator, 
        ILogger logger,
        ActivitySource source)
    {
        var personResponse = await TryUpdatePerson(request, mapper, mediator, logger, source);
        var shapedPersonResponseWithLinks = PersonResponseHelper.ShapePersonWithLinks(personResponse);
        return Results.Ok(shapedPersonResponseWithLinks);
    }

    private async static Task<PersonResponse> TryAddPerson(
        UpdatePersonRequest request,
        IMapper mapper,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        try
        {
            var added = await PersonHelper.AddPerson(request, mapper, mediator, logger, source);
            var response = PersonResponseHelper.From(added, mapper);
            return response;
        }
        catch (Exception e)
        {
            logger.Error(e, "Error occurred whilst trying to add a person!");
            throw;
        }
    }

    private async static Task<PersonResponse> TryUpdatePerson(
        UpdatePersonRequest request,
        IMapper mapper,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        try
        {
            var updated = await PersonHelper.UpdatePerson(request, mapper, mediator, logger);
            var response = PersonResponseHelper.From(updated, mapper);
            return response;
        }
        catch (Exception e)
        {
            logger.Error(e, "Error occurred whilst trying to delete a person!");
            throw;
        }
    }
}