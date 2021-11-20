namespace Users.Application.Commands;

public class DeleteUserCommand : IRequest
{
    public Guid Id { get; set; }
}
