namespace Users.API.Models.Shared;

public class PropertyMapping<TSource, TDestination> : IPropertyMapping
{
    public Dictionary<string, PropertyMappingValue> _mappings { get; set; }

    public PropertyMapping(Dictionary<string, PropertyMappingValue> mappings)
    {
        if (mappings == null) throw new ArgumentNullException(nameof(mappings));

        _mappings = mappings;
    }
}
