namespace Morsley.UK.People.API.Contracts.Validators;

public class GetPersonRequestValidator : AbstractValidator<GetPersonRequest>
{
    public GetPersonRequestValidator(IPropertyMappings propertyMappings)
    {
        if (propertyMappings is null) throw new ArgumentNullException(nameof(propertyMappings));

        PropertyMappings = propertyMappings;

        RuleFor(_ => _.Fields).Must(BeValidFields).WithMessage($"The fields value is invalid. e.g. fields=id,lastname");
    }

    public IPropertyMappings PropertyMappings { get; }

    protected bool BeValidFields(string? fields)
    {
        if (fields is null) return true;

        return PropertyMappings.DoesValidMappingExistFor<GetPersonRequest, Person>(fields);
    }
}
