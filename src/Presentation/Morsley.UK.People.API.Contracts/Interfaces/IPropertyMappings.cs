using Morsley.UK.People.API.Contracts.Shared;

namespace Morsley.UK.People.API.Contracts.Interfaces;

public interface IPropertyMappings
{    
    public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();

    public bool DoesValidMappingExistFor<TSource, TDestination>(string fields);
}