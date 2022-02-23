namespace Users.API.Models.Request.v1;

/// <summary>
/// The object used to request the deletion of a user.
/// </summary>
public class DeleteUserRequest
{

    public DeleteUserRequest(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// The unique identifier of the user to be deleted.
    /// </summary>
    public Guid Id { get; }
}
