namespace Morsley.UK.People.API.Contracts.Requests;

public record AddPersonRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }

    public string? DateOfBirth { get; set; }
}
