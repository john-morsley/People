namespace Users.API.Models.Request.v1;

public class GetUserRequest
{
    public GetUserRequest(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; private set; }
}
