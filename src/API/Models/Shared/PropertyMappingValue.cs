namespace Users.API.Models.Shared;

public class PropertyMappingValue
{
    public IEnumerable<string> DestinationProperties { get; private set; }

    public bool Revert { get; private set; }

    public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
    {
        if (destinationProperties == null) throw new ArgumentNullException(nameof(destinationProperties));

        DestinationProperties = destinationProperties;
        Revert = revert;
    }
}
