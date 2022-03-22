namespace Morsley.UK.People.API.Contracts.Validators;

public class DeletePersonRequestValidator : AbstractValidator<DeletePersonRequest>
{
    public DeletePersonRequestValidator()
    {
        RuleFor(_ => _.Id).Must(BeValid).WithMessage("Id cannot be its default value.");
    }

    private bool BeValid(Guid id)
    {
        if (id == Guid.Empty) return false;
        return true;
    }
}
