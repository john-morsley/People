namespace Users.API.Models.Extensions;

public static class GetUserResponseExtensions
{
    public static ExpandoObject ShapeData(this UserResponse source, string fields)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var shaped = new ExpandoObject();

        var propertiesToInclude = new List<PropertyInfo>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            var propertyInfos = typeof(UserResponse).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            propertiesToInclude.AddRange(propertyInfos);
        }
        else
        {
            var splitFields = fields.Split(',');

            foreach(var field in splitFields)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(UserResponse).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null) throw new ArgumentOutOfRangeException(nameof(fields), $"Field '{propertyName}' was not found.");
                propertiesToInclude.Add(propertyInfo);
            }
        }

        foreach(var propertyToInclude in propertiesToInclude)
        {
            var name = propertyToInclude.Name;
            var value = propertyToInclude.GetValue(source);
            shaped.TryAdd(name, value);
        }

        return shaped;
    }
}
