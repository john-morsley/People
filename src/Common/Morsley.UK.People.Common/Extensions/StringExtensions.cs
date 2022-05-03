namespace Morsley.UK.People.Common.Extensions;

public static class StringExtensions
{
    public static string GetDisplayValue(this string? value)
    {
        if (value is null) return "[NULL]";
        if (value.Length == 0) return "[EMPTY]";
        if (value.IsWhiteSpace()) return "[WHITESPACE]";
        return value;
    }

    public static bool IsWhiteSpace(this string value)
    {
        if (value.Length == 0) return false;

        var characters = value.ToCharArray();
        foreach (var character in characters)
        {
            if (!char.IsWhiteSpace(character)) return false;
        }
        return true;
    }
}
