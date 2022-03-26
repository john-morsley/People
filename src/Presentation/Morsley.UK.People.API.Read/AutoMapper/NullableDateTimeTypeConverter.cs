namespace Morsley.UK.People.API.Read.AutoMapper;

/// <summary>
/// A type converter that converts a nullable DateTime to a nullable string..
/// </summary>
public class NullableDateTimeTypeConverter : ITypeConverter<DateTime?, string?>
{
    /// <summary>
    /// Convert a nullable DateTime into a nullable string.
    /// </summary>
    /// <param name="source">The nullable DateTime to be converted.</param>
    /// <param name="destination"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string? Convert(
        DateTime? source,
        string? destination,
        ResolutionContext context)
    {
        if (source == null) return null;

        var conversion = $"{source.Value.Year:0000}-{source.Value.Month:00}-{source.Value.Day:00}";

        return conversion;
    }
}