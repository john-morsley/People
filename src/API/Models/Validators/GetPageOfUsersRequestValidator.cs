namespace Users.API.Models.v1.Validators
{
    public class GetPageOfUsersRequestValidator : AbstractValidator<Users.API.Models.Request.v1.GetPageOfUsersRequest>
    {
        public GetPageOfUsersRequestValidator()
        {
            RuleFor(_ => _.OrderBy).Must(BeValidSort).WithMessage("The sort order is invalid.");
            RuleFor(_ => _.Filter).Must(BeValidFilter).WithMessage("The filter is invalid.");
        }
         
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

        protected bool BeValidFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return true;

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
            if (field == "dateofbirth") return true;
            if (field == "gender") return true;
            if (field == "firstname") return true;
            if (field == "lastname") return true;
            if (field == "sex") return true;
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
            if (field == "dateofbirth") return true;
            if (field == "firstname") return true;
            if (field == "lastname") return true;            
            return false;
        }
    }
}