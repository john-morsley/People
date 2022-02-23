namespace Users.Application.Queries;

public sealed class UserExistsQuery : IRequest<bool>
{
    public UserExistsQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; private set; }
}
