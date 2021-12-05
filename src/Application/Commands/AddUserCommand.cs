namespace Users.Application.Commands;

public sealed class AddUserCommand : IRequest<Users.Domain.Models.User>
{
    //public Users.Domain.Enumerations.Title? Title { get; set; }

    public AddUserCommand()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    //public Users.Domain.Enumerations.Sex? Sex { get; set; }
}
