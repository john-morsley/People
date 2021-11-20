namespace Users.Application.Commands;

public sealed class PartiallyUpdateUserCommand : IRequest<Users.Domain.Models.User>
{
    public Guid Id { get; set; }

    public Users.Domain.Enumerations.Title? Title { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Users.Domain.Enumerations.Sex? Sex { get; set; }

    public bool TitleChanged { get; set; }

    public bool FirstNameChanged { get; set; }

    public bool LastNameChanged { get; set; }

    public bool SexChanged { get; set; }
}