namespace Morsley.UK.People.Domain.Extensions;

public static class GenderExtensions
{
    public static string GetDisplayValue(this Gender? value)
    {
        if (value is null) return "[NULL]";
        return value.ToString();
    }
}