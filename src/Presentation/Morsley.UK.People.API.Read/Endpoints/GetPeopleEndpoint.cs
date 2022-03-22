using FluentValidation;
using Morsley.UK.People.API.Contracts.Requests;
using Morsley.UK.People.API.Contracts.Responses;
using Morsley.UK.People.API.Read.Services;

namespace Morsley.UK.People.API.Read.Endpoints;

/// <summary>
/// 
/// </summary>
public static class GetPeopleEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public static void MapGetPeopleEndpoint(this WebApplication application)
    {
        application.MapGet("/api/people", async (
                    GetPeopleRequest request,
                    IValidator<GetPeopleRequest> validator,
                    IMapper mapper,
                    IMediator mediator,
                    ILogger logger)
                    =>
                    await GetPeople(request, validator, mapper, mediator, logger))
                   .Accepts<GetPeopleRequest>("application/json")
                   .Produces<IPagedList<PersonResponse>>(StatusCodes.Status200OK, "application/json")
                   .Produces(StatusCodes.Status422UnprocessableEntity)
                   .Produces(StatusCodes.Status500InternalServerError)
                   .WithName("GetPeople");
    }

    private async static Task<IResult> GetPeople(
        GetPeopleRequest request,
        IValidator<GetPeopleRequest> validator,
        IMapper mapper, 
        IMediator mediator, 
        ILogger logger)
    {
        if (!ValidatorHelper.IsRequestValid(request, validator, out var problemDetails)) return Results.UnprocessableEntity(problemDetails);

        var pageOfPersonResponses = await PeopleService.TryGetPeople(request, mapper, mediator, logger);

        if (pageOfPersonResponses.Count == 0) return Results.NoContent();

        // Shape people...
        var shapedPageOfPersons = pageOfPersonResponses.ShapeData(request.Fields);

        // Add Metadata links...         
        var shapedPageOfPersonsWithLinks = LinksHelper.AddLinks(shapedPageOfPersons);

        var pageOfPersonsLinks = LinksHelper.CreateLinksForPageOfPersons(request, pageOfPersonResponses);

        var shapedPageOfPersonsWithLinksWithSelfLink = new ExpandoObject() as IDictionary<string, object>;
        shapedPageOfPersonsWithLinksWithSelfLink.Add("_embedded", shapedPageOfPersonsWithLinks);
        shapedPageOfPersonsWithLinksWithSelfLink.Add("_links", pageOfPersonsLinks);

        return Results.Ok(shapedPageOfPersonsWithLinksWithSelfLink);
    }

    private static void ValidateRequest(GetPeopleRequest request, IValidator<GetPeopleRequest> validator)
    {
        var validatioResult = validator.Validate(request);

        if (validatioResult.IsValid) return;

        //var errors = validatioResult.Errors();
    }
}