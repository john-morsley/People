using System.Linq;

namespace Users.API.Models.Shared
{
    public class PropertyMappings : IPropertyMappings
    {
        private Dictionary<string, PropertyMappingValue> _userPropertyMappings =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" }) },
                { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" }) },
                { "LastName", new PropertyMappingValue(new List<string>() { "LastName" }) },
                { "Sex", new PropertyMappingValue(new List<string>() { "Sex" }) },
                { "Gender", new PropertyMappingValue(new List<string>() { "Gender" }) },
                { "DateOfBirth", new PropertyMappingValue(new List<string>() { "DateOfBirth" }) },
            };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappings()
        {
            _propertyMappings.Add(new PropertyMapping<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>(_userPropertyMappings));
        }

        public void Add(string sourcePropertyName, PropertyMappingValue destinationPropertyNames)
        {
            _userPropertyMappings.Add(sourcePropertyName, destinationPropertyNames);
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMappings = _propertyMappings.OfType<PropertyMapping<Users.API.Models.Request.v1.GetUserRequest, Users.Domain.Models.User>>();

            if (matchingMappings.Count() != 1) throw new Exception($"Cannot find exact property mapping instance for '<{typeof(TSource)},{typeof(TDestination)}>'");

            return matchingMappings.First()._mappings;
        }

        public bool DoesValidMappingExistFor<TSource, TDestination>(string fields)
        {
            var propertyMappings = GetPropertyMapping<TSource, TDestination>();

            if (fields == null) return true;

            var splitFields = fields.Split(',');
            foreach (var field in splitFields)
            {
                var fieldName = field.Trim();
                if (!propertyMappings.ContainsKey(fieldName)) return false;
            }

            return true;
        }
    }
}
