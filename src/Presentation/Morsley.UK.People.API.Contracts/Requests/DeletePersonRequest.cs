namespace Morsley.UK.People.API.Contracts.Requests;

public record DeletePersonRequest
{
    public Guid Id { get; set; }

    public static ValueTask<DeletePersonRequest> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        var potentialId = httpContext.Request.RouteValues.SingleOrDefault(_ => _.Key == "id").Value;
        if (potentialId == null) throw new ArgumentOutOfRangeException("id", "Expected id is missing!");

        Guid.TryParse(potentialId.ToString(), out var id);

        var request = new DeletePersonRequest
        {
            Id = id
        };

        return ValueTask.FromResult<DeletePersonRequest>(request);
    }

    public static bool TryParse(string value) => true;
}