namespace Users.Application.Commands;

public sealed class AddUserCommand : IRequest<Guid>
{
    public Users.Domain.Enumerations.Title? Title { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Users.Domain.Enumerations.Sex? Sex { get; set; }
}
