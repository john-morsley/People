namespace Morsley.UK.People.API.Contracts.Requests;

public record GetPersonRequest
{
    public Guid Id { get; set; }

    public string? Fields { get; set; }

    public static ValueTask<GetPersonRequest> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        //Guid.TryParse(httpContext.Request.RouteValues["id"], out var id);
        var fields = httpContext.Request.Query["fields"];

        var potentialId = httpContext.Request.RouteValues.SingleOrDefault(_ => _.Key == "id").Value;
        if (potentialId == null) throw new ArgumentOutOfRangeException("id", "Expected id is missing!");

        Guid.TryParse(potentialId.ToString(), out var id);

        var request = new GetPersonRequest
        {
            Id = id,
            Fields = fields
        };

        return ValueTask.FromResult<GetPersonRequest>(request);
    }

    public static bool TryParse(string value, out object request)
    {
        request = new GetPeopleRequest();
        return true;
    }
}