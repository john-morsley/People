namespace Users.API.Models.v1.Validators
{
    public class GetUserRequestValidator : AbstractValidator<Users.API.Models.Request.v1.GetUserRequest>
    {
        public GetUserRequestValidator(IPropertyMappings propertyMappings)
        {
            if (propertyMappings == null) throw new ArgumentNullException(nameof(propertyMappings));

            PropertyMappings = propertyMappings;

            RuleFor(_ => _.Fields).Must(BeValidFields).WithMessage("The field(s) are invalid.");            
        }

        public IPropertyMappings PropertyMappings { get; }

        protected bool BeValidFields(string fields)
        {
            if (fields == null) return true;

            var splitFields = fields.Split(',');
            foreach (var field in splitFields)
            {
                var fieldName = field.Trim();

                if (!IsFieldValid(fieldName)) return false;
            }

            return true;
        }

        private bool IsFieldValid(string field)
        {
            if (string.IsNullOrEmpty(field)) return false;
            field = field.ToLower();

            var userMappings = PropertyMappings.GetPropertyMapping<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>();
            if (userMappings.ContainsKey(field)) return true;

            return false;
        }
    }
}