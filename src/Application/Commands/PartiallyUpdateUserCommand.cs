namespace Users.Application.Commands;

public sealed class PartiallyUpdateUserCommand : IRequest<Users.Domain.Models.User>
{
    public Guid Id { get; set; }

    //public Users.Domain.Enumerations.Title? Title { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }

    //public bool TitleChanged { get; set; }

    public bool FirstNameChanged { get; set; }

    public bool LastNameChanged { get; set; }

    public bool SexChanged { get; set; }

    public bool GenderChanged { get; set; }
}