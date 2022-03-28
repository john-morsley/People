

namespace Morsley.UK.People.Tests.Common;

public static class StringExtensions
{
    public static string? GenerateDifferentDate(this string? initialDate)
    {
        DateTime? newDate;
        if (initialDate is not null && initialDate.Length != 10) throw new InvalidOperationException("An international date must be 10 digits in length!");
        if (initialDate is null)
        {
            DateTime? nullableDateTime = null;
            newDate = nullableDateTime.GenerateDifferentDate();
            return newDate!.Value.ToString("yyyy-MM-dd");
        }

        if (!DateTime.TryParseExact(initialDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt))
        {
            throw new InvalidOperationException("Expected an international date: YYYY-MM-DD!");
        }

        newDate = dt.GenerateDifferentDate();
        return newDate!.Value.ToString("yyyy-MM-dd");
    }
}
