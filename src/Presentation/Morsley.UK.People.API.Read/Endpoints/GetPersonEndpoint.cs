using FluentValidation;
using System.Linq;

namespace Morsley.UK.People.API.Read.Endpoints;

/// <summary>
/// 
/// </summary>
public static class GetPersonEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapGetPersonEndpoint(this WebApplication application)
    {
        application.MapMethods("/api/person/{id}", new [] { "GET" }, async (
                    GetPersonRequest request,
                    IValidator<GetPersonRequest> validator,
                    IMapper mapper,
                    IMediator mediator,
                    ILogger logger)
                    =>
                    await GetPerson(request, validator, mapper, mediator, logger))
                   .Accepts<GetPersonRequest>("application/json")
                   .Produces<PersonResponse>(StatusCodes.Status200OK, "application/json")
                   .Produces(StatusCodes.Status400BadRequest)
                   .Produces(StatusCodes.Status204NoContent)
                   .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, "application/problem+json")
                   .WithName("GetPerson"); //.AllowAnonymous();
    }

    private async static Task<IResult> GetPerson(
        GetPersonRequest request,
        IValidator<GetPersonRequest> validator,
        IMapper mapper,
        IMediator mediator,
        ILogger logger)
    {
        if (!ValidatorHelper.IsRequestValid(request, validator, out var problemDetails)) return Results.UnprocessableEntity(problemDetails);

        if (request.Id == Guid.Empty) return Results.BadRequest();

        var personResponse = await PersonService.TryGetPerson(request, mapper, mediator, logger);

        if (personResponse == null) return Results.NoContent();

        var shapedPerson = personResponse.ShapeData(request.Fields);

        var shapedPersonWithLinks = LinksHelper.AddLinks(shapedPerson!, personResponse.Id);

        return Results.Ok(shapedPersonWithLinks);
    }
}