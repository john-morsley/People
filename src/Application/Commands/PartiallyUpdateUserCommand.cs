namespace Users.Application.Commands;

public sealed class PartiallyUpdateUserCommand : IRequest<Users.Domain.Models.User>
{
    public Guid Id { get; set; }

    //public Users.Domain.Enumerations.Title? Title { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }
}