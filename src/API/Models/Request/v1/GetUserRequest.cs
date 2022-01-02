namespace Users.API.Models.Request.v1;

public class GetUserRequest
{
    public Guid Id { get; set; }

    public string Fields { get; set; }
}
