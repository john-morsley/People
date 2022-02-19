namespace Users.API.Models.v1.Validators
{
    public class GetUserRequestValidator : AbstractValidator<Users.API.Models.Request.v1.GetUserRequest>
    {
        public GetUserRequestValidator(IPropertyMappings propertyMappings)
        {
            if (propertyMappings == null) throw new ArgumentNullException(nameof(propertyMappings));

            PropertyMappings = propertyMappings;

            RuleFor(_ => _.Fields).Must(BeValidFields).WithMessage("The fields value is invalid. e.g. fields=id,lastname");
        }

        public IPropertyMappings PropertyMappings { get; }

        protected bool BeValidFields(string? fields)
        {
            if (fields == null) return true;

            return PropertyMappings.DoesValidMappingExistFor<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>(fields);
        }

        //private bool IsFieldValid(string field)
        //{
        //    if (string.IsNullOrEmpty(field)) return false;
        //    field = field.ToLower();

        //    //var userMappings = PropertyMappings.GetPropertyMapping<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>();
        //    //if (userMappings.ContainsKey(field)) return true;

        //    //if (PropertyMappings.DoesValidMappingExistFor<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>()

        //    return false;
        //}
    }
}