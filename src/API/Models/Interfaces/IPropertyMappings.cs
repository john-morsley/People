namespace Users.API.Models.Interfaces;

public interface IPropertyMappings
{    
    public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
}