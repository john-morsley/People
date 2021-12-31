namespace Users.API.Models.v1.Validators
{
    public class AddUserRequestValidator : AbstractValidator<Users.API.Models.Request.v1.AddUserRequest>
    {
        public AddUserRequestValidator()
        {
            //var blacklistedCharacters = configuration["BlacklistedCharacters"];

            //RuleFor(x => x.Title)
            //    .IsEnumName(typeof(Title), caseSensitive: false);

            RuleFor(_ => _.FirstName).NotEmpty().WithMessage("First name cannot be empty.")
                                     .Length(1, 100).WithMessage("First name cannot exceed 100 characters.");

            RuleFor(_ => _.LastName).NotEmpty().WithMessage("Last name cannot be empty.")
                                    .Length(1, 100).WithMessage("Last name cannot exceed 100 characters.");

            //RuleFor(x => x.Sex)
            //    .IsEnumName(typeof(Sex), caseSensitive: false);
        }
    }
}