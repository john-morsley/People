namespace Morsley.UK.People.Application.Validators;

public class AddPersonCommandValidator : AbstractValidator<AddPersonCommand>
{
    public AddPersonCommandValidator()
    {
        RuleFor(_ => _.FirstName).NotEmpty();
        RuleFor(_ => _.LastName).NotEmpty();
    }
}
