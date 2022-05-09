namespace Morsley.UK.People.Domain.Extensions;

public static class SexExtensions
{
    public static string GetDisplayValue(this Sex? value)
    {
        if (value is null) return "[NULL]";
        return value.ToString();
    }
}