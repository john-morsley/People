namespace Users.API.Models.Request.v1;

public class PartiallyUpdateUserRequest
{
    public PartiallyUpdateUserRequest(Guid id)
    {
        Id = id;
    }
        
    public Guid Id { get; protected set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }
}
