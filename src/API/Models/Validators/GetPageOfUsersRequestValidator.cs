namespace Users.API.Models.v1.Validators
{
    public class GetPageOfUsersRequestValidator : AbstractValidator<Users.API.Models.Request.v1.GetPageOfUsersRequest>
    {
        public const string ValidSortFields = "Id,FirstName,LastName,Sex,Gender,DateOfBirth";
        public const string ValidFilterFields = "Sex,Gender,Date";

        public GetPageOfUsersRequestValidator(IPropertyMappings propertyMappings)
        {
            if (propertyMappings == null) throw new ArgumentNullException(nameof(propertyMappings));

            PropertyMappings = propertyMappings;

            RuleFor(_ => _.Fields).Must(BeValidFields).WithMessage("The fields value is invalid. e.g. fields=id,lastname");
            RuleFor(_ => _.Filter).Must(BeValidFilter).WithMessage("The filter value is invalid. e.g. filter=sex:male");
            RuleFor(_ => _.Sort).Must(BeValidSort).WithMessage($"The sort value is invalid. e.g. sort={Users.API.Models.Constants.Defaults.DefaultPageSort}");
        }

        public IPropertyMappings PropertyMappings { get; }

        protected bool BeValidSort(string sort)
        {
            if (string.IsNullOrEmpty(sort)) return true;

            var fields = new List<string>();

            var values = sort.Split(',');
            foreach (var value in values)
            {
                var kvp = value.Split(':');
                if (kvp.Length < 1 || kvp.Length > 2) return false;
                if (kvp.Length == 1 )
                {
                    var field = kvp[0].ToLower();
                    if (field == "id") return false;
                    fields.Add(field);
                    //if (!IsSortFieldValid(field)) return false;

                }
                if (kvp.Length == 2)
                {
                    var field = kvp[0].ToLower();
                    if (field == "id") return false;
                    fields.Add(field);
                    //if (!IsSortFieldValid(field)) return false;
                    var direction = kvp[1];
                    if (!IsSortDirectionValid(direction)) return false;
                }
            }            

            //return true;
            return PropertyMappings.DoesValidMappingExistFor<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>(string.Join(",", fields));
        }

        protected bool BeValidFields(string fields)
        {
            if (fields == null) return true;

            return PropertyMappings.DoesValidMappingExistFor<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>(fields);
        }

        protected bool BeValidFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return true;

            //var userMappings = PropertyMappings.GetPropertyMapping<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>();

            var fields = new List<string>();

            var values = filter.Split(',');
            foreach (var value in values)
            {
                var kvp = value.Split(':');
                if (kvp.Length != 2) return false;
                var field = kvp[0].ToLower();
                if (field == "id") return false;
                fields.Add(field);
                //if (!IsFilterFieldValid(field)) return false;
            }

            //return true;
            return PropertyMappings.DoesValidMappingExistFor<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>(string.Join(",", fields));
        }

        //private bool IsFilterFieldValid(string field)
        //{
        //    if (string.IsNullOrEmpty(field)) return false;
        //    field = field.ToLower();

        //    var userMappings = PropertyMappings.GetPropertyMapping<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>();
        //    if (userMappings.ContainsKey(field)) return true;

        //    return false;
        //}

        private bool IsSortDirectionValid(string direction)
        {
            if (string.IsNullOrEmpty (direction)) return false;
            direction = direction.ToLower();
            if (direction == "asc" || direction == "ascending") return true;
            if (direction == "desc" || direction == "descending") return true;
            return false;
        }

        //private bool IsSortFieldValid(string field)
        //{
        //    if (string.IsNullOrEmpty(field)) return false;
        //    field = field.ToLower();

        //    if (field == "id") return false;

        //    return PropertyMappings.DoesValidMappingExistFor<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>(fields);

        //    return false;
        //}

        //private bool IsFieldValid(string field)
        //{
        //    if (string.IsNullOrEmpty(field)) return false;
        //    field = field.ToLower();

        //    var userMappings = PropertyMappings.GetPropertyMapping<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>();
        //    if (userMappings.ContainsKey(field)) return true;

        //    return false;
        //}
    }
}