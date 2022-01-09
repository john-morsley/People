namespace Users.API.Models.Interfaces;

public interface IPropertyMappings
{    
    public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();

    public bool DoesValidMappingExistFor<TSource, TDestination>(string fields);
}