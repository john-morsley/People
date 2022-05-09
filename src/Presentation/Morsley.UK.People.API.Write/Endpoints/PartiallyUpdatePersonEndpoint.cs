namespace Morsley.UK.People.API.Write.Endpoints;

/// <summary>
/// 
/// </summary>
public static class PartiallyUpdatePersonEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapPartiallyUpdatePersonEndpoint(this WebApplication application)
    {
        application.MapMethods("/api/person/{id}", new [] { "PATCH" }, async (
                    [FromRoute] Guid id,
                    [FromBody] JsonElement body,
                    [FromServices] IValidator<UpdatePersonRequest> validator,
                    [FromServices] IMapper mapper,
                    [FromServices] IMediator mediator,
                    [FromServices] ILogger logger,
                    [FromServices] ActivitySource source)
                   => 
                   await UpsertPerson(id, body, validator, mapper, mediator, logger, source))
                   .Accepts<JsonPatchDocument<UpdatePersonRequest>>("application/json")
                   .Produces<string>(StatusCodes.Status200OK, "application/json")
                   .WithName("PartiallyUpdatePerson");
    }

    [HttpPatch]
    private async static Task<IResult> UpsertPerson(
        Guid id, 
        JsonElement body,
        IValidator<UpdatePersonRequest> validator,
        IMapper mapper,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        JsonPatchDocument? updates = null;
        try
        {
            //if (body == null) return Results.BadRequest();

            updates = DetermineUpdates(body);

            // As we are upserting, we either update an existing person or we add a new person...
            if (await PersonHelper.DoesPersonExist(id, mediator, logger))
            {
                // Update existing person...
                return await UpdatePerson(id, validator, mapper, mediator, logger, updates);
            }

            // Add a new person...
            return await AddPerson(id, validator, mapper, mediator, logger, updates, source);
        }
        catch (UnprocessableEntityException uee)
        {
            logger.Error("An unexpected error occurred whilst attempting to upsert a person!", uee);
            var errorProblemDetails = new ProblemDetails {
                Title = "Unexpected error(s) occurred!",
                Instance = "",
                Detail = "See the errors field for details.",
                Type = string.Empty,
                Status = StatusCodes.Status422UnprocessableEntity
            };
            if (updates != null && updates.Operations != null)
            {
                var i = 1;
                foreach (var operation in updates.Operations)
                {
                    var op = operation.op;
                    var path = operation.path ?? "[Node]";
                    var value = operation.value ?? "[None]";
                    errorProblemDetails.Extensions.Add($"Operation_{i++}", $"Op: '{op}' - Path: '{path}' - Value: '{value}'");
                }
            }
            return Results.UnprocessableEntity(errorProblemDetails);

        }
        catch (Exception e)
        {
            logger.Error("An unexpected error occurred whilst attempting to upsert a person!", e);
            return Results.Problem(statusCode: 500);
        }
    }

    private async static Task<IResult> AddPerson(
        Guid id, 
        IValidator<UpdatePersonRequest> validator, 
        IMapper mapper, 
        IMediator mediator, 
        ILogger logger,
        JsonPatchDocument updates,
        ActivitySource source)
    {
        var request = new UpdatePersonRequest { Id = id };
        try
        {
            updates.ApplyTo(request);
            if (!ValidatorHelper.IsRequestValid(request, validator, out var addProblemDetails)) return Results.UnprocessableEntity(addProblemDetails);
        }
        catch (Exception e)
        {
            logger.Error("An unexpected error occurred whilst attempting to add a person! (Suspected unprocessable entity)", e);
            throw new UnprocessableEntityException("An unexpected error occurred whilst attempting to add a person! (Suspected unprocessable entity)", e);
        }

        try
        {
            var personResponse = await AddPerson(request, mapper, mediator, logger, source);
            var shapedPersonResponseWithLinks = PersonResponseHelper.ShapePersonWithLinks(personResponse);
            return Results.Created($"https://localhost/api/person/{id}", shapedPersonResponseWithLinks);
        }
        catch (Exception e)
        {
            logger.Error("An unexpected error occurred whilst attempting to add a person!", e);
            throw;
        }
    }

    private async static Task<PersonResponse> AddPerson(
        UpdatePersonRequest request,
        IMapper mapper,
        IMediator mediator,
        ILogger logger,
        ActivitySource source)
    {
        var added = await PersonHelper.AddPerson(request, mapper, mediator, logger, source);
        var response = PersonResponseHelper.From(added, mapper);
        return response;
    }

    private static JsonPatchDocument DetermineUpdates(JsonElement body)
    {
        var json = body.GetRawText();
        var reader = new StringReader(json);
        var jsonReader = new Newtonsoft.Json.JsonTextReader(reader);
        var converter = new JsonPatchDocumentConverter();
        var updates = converter.ReadJson(jsonReader, typeof(JsonPatchDocument), existingValue: null, new Newtonsoft.Json.JsonSerializer()) as JsonPatchDocument;
        if (updates == null) throw new InvalidOperationException("Expected the body to contain a JSON patch document!");
        return updates;
    }

    private async static Task<IResult> UpdatePerson(
        Guid id, 
        IValidator<UpdatePersonRequest> validator, 
        IMapper mapper, 
        IMediator mediator,
        ILogger logger, 
        JsonPatchDocument updates)
    {
        var person = await PersonHelper.GetPerson(id, mapper, mediator, logger);
        var request = mapper.Map<UpdatePersonRequest>(person);
        try
        {
            updates.ApplyTo(request);
            if (!ValidatorHelper.IsRequestValid(request, validator, out var updateProblemDetails)) return Results.UnprocessableEntity(updateProblemDetails);
        }
        catch (Exception e)
        {
            logger.Error("An unexpected error occurred whilst attempting to update a person! (Suspected unprocessable entity)", e);
            throw new UnprocessableEntityException("An unexpected error occurred whilst attempting to update a person! (Suspected unprocessable entity)", e);
        }

        try
        {
            var personResponse = await UpdatePerson(request, mapper, mediator, logger);
            var shapedPersonResponseWithLinks = PersonResponseHelper.ShapePersonWithLinks(personResponse);
            return Results.Ok(shapedPersonResponseWithLinks);
        }
        catch (Exception e)
        {
            logger.Error("An unexpected error occurred whilst attempting to update a person!", e);
            throw;
        }
    }

    private async static Task<PersonResponse> UpdatePerson(
        UpdatePersonRequest request,
        IMapper mapper,
        IMediator mediator,
        ILogger logger)
    {
        var updated = await PersonHelper.UpdatePerson(request, mapper, mediator, logger);
        var response = PersonResponseHelper.From(updated, mapper);
        return response;
    }
}
