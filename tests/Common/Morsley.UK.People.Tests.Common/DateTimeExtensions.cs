namespace Morsley.UK.People.Tests.Common;

public static class DateExtensions
{
    public static DateTime? GenerateDifferentDate(this DateTime? initialDate)
    {
        var af = new Fixture();
        DateTime? differentDate;
        do
        {
            DateTime? newDate;
            if (initialDate is null)
            {
                newDate = af.Create<DateTime>();
            }
            else
            {
                newDate = af.Create<DateTime?>();
            }
            differentDate = new DateTime(newDate!.Value.Year, newDate!.Value.Month, newDate!.Value.Day, 0, 0, 0, DateTimeKind.Utc);
        }
        while (initialDate == differentDate);

        return differentDate;
    }

    public static DateTime GenerateDifferentDate(this DateTime initialDate)
    {
        var af = new Fixture();
        DateTime differentDate;
        do
        {
            var newDate = af.Create<DateOnly>();
            differentDate = new DateTime(newDate.Year, newDate!.Month, newDate.Day, 0, 0, 0, DateTimeKind.Utc);
        }
        while (initialDate == differentDate);

        return differentDate;
    }
}
