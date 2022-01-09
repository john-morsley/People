namespace Users.API.Models.Shared;

public class PropertyMappingValue
{
    public IEnumerable<string> DestinationProperties { get; private set; }

    public bool RevertSort { get; private set; }

    public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revertSort = false)
    {
        if (destinationProperties == null) throw new ArgumentNullException(nameof(destinationProperties));

        DestinationProperties = destinationProperties;
        RevertSort = revertSort;
    }
}
