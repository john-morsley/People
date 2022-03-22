namespace Morsley.UK.People.API.Write.AutoMapper;

/// <summary>
/// 
/// </summary>
public class DateTimeToStringConverter : ITypeConverter<DateTime?, string?>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string? Convert(DateTime? source, string? destination, ResolutionContext context)
    {
        if (source == null) return null;

        var dt = source.Value;

        // Date Only...
        if (dt.Hour == 0 && dt.Minute == 0 && dt.Hour == 0) return dt.ToString("yyyy-MM-dd");

        // Time only....
        if (dt.Year == 0 && dt.Month == 0 && dt.Day == 0) return dt.ToString("HH:mm:ss");

        // Both date and time...
        return dt.ToString("yyyy-MM-dd HH:mm:ss");

    }
}