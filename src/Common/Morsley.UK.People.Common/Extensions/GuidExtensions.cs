namespace Morsley.UK.People.Common.Extensions;

public static class GuidExtensions
{
    public static string GetDisplayValue(this Guid? value)
    {
        if (value is null) return "[NULL]";
        if (value == Guid.Empty) return "[EMPTY]";
        return value.ToString();
    }
}
