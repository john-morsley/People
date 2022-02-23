namespace Users.Application.Commands;

public sealed class PartiallyUpdateUserCommand : IRequest<Users.Domain.Models.User>
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }

    public string? DateOfBirth { get; set; }
}