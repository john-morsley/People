namespace Users.API.Models.v1.Validators
{
    public class GetPageOfUsersRequestValidator : AbstractValidator<Users.API.Models.Request.v1.GetPageOfUsersRequest>
    {
        public GetPageOfUsersRequestValidator(IPropertyMappings propertyMappings)
        {
            if (propertyMappings == null) throw new ArgumentNullException(nameof(propertyMappings));

            PropertyMappings = propertyMappings;

            RuleFor(_ => _.OrderBy).Must(BeValidSort).WithMessage("The sort order is invalid.");
            RuleFor(_ => _.Filter).Must(BeValidFilter).WithMessage("The filter is invalid.");
            RuleFor(_ => _.Fields).Must(BeValidFields).WithMessage("The field(s) are invalid.");
        }

        public IPropertyMappings PropertyMappings { get; }

        protected bool BeValidSort(string sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder)) return true;

            var values = sortOrder.Split(',');
            foreach (var value in values)
            {
                var kvp = value.Split(':');
                if (kvp.Length < 1 || kvp.Length > 2) return false;
                if (kvp.Length == 1 )
                {
                    var field = kvp[0];
                    if (!IsSortFieldValid(field)) return false;

                }
                if (kvp.Length == 2)
                {
                    var field = kvp[0];
                    if (!IsSortFieldValid(field)) return false;
                    var direction = kvp[1];
                    if (!IsSortDirectionValid(direction)) return false;
                }
            }

            return true;
        }

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

        protected bool BeValidFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return true;

            var userMappings = PropertyMappings.GetPropertyMapping<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>();

            var values = filter.Split(',');
            foreach (var value in values)
            {
                var kvp = value.Split(':');
                if (kvp.Length != 2) return false;
                var field = kvp[0];
                if (!IsFilterFieldValid(field)) return false;
            }

            return true;
        }

        private bool IsFilterFieldValid(string field)
        {
            if (string.IsNullOrEmpty(field)) return false;
            field = field.ToLower();

            var userMappings = PropertyMappings.GetPropertyMapping<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>();
            if (userMappings.ContainsKey(field)) return true;

            return false;
        }

        private bool IsSortDirectionValid(string direction)
        {
            if (string.IsNullOrEmpty (direction)) return false;
            direction = direction.ToLower();
            if (direction == "asc" || direction == "ascending") return true;
            if (direction == "desc" || direction == "desscending") return true;
            return false;
        }

        private bool IsSortFieldValid(string field)
        {
            if (string.IsNullOrEmpty(field)) return false;
            field = field.ToLower();

            var userMappings = PropertyMappings.GetPropertyMapping<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>();
            if (userMappings.ContainsKey(field)) return true;

            return false;
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