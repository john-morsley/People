namespace Users.API.Models.Request.v1;

public class PartiallyUpdateUserRequest
{
    public PartiallyUpdateUserRequest(Guid id)
    {
        Id = id;
    }
        
    public Guid Id { get; protected set; }
}
