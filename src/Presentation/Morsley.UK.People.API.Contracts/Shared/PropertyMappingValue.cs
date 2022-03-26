namespace Morsley.UK.People.API.Contracts.Shared;

public class PropertyMappingValue
{
    public IEnumerable<string> DestinationProperties { get; private set; }

    public bool RevertSort { get; private set; }

    public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revertSort = false)
    {
        DestinationProperties = destinationProperties ?? throw new ArgumentNullException(nameof(destinationProperties));
        RevertSort = revertSort;
    }
}