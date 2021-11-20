namespace Users.API.Models.Request.v1;

public class PartiallyUpsertUserRequest
{
    public PartiallyUpsertUserRequest(Guid id)
    {
        Id = id;
    }
        
    public Guid Id { get; protected set; }
}
