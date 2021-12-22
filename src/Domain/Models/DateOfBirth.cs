namespace Users.Domain.Models;

public class DateOfBirth
{
    public DateOfBirth(int year, int month, int day)
    {
        if (year < YearMinimumValue) throw new ArgumentOutOfRangeException(nameof(year), $"Year out of range - Minimum value: {YearMinimumValue}");
        if (year > YearMaximumValue) throw new ArgumentOutOfRangeException(nameof(year), $"Year out of range - Maximum value: {YearMaximumValue}");
        if (month < 1) throw new ArgumentOutOfRangeException(nameof(month), $"Month out of range - Minimum value: 1");
        if (month > 12) throw new ArgumentOutOfRangeException(nameof(month), $"Month out of range - Maximum value: 12");
        if (day < 1) throw new ArgumentOutOfRangeException(nameof(day), $"Day out of range - Minimum value: 1");
        if (day > 31) throw new ArgumentOutOfRangeException(nameof(day), $"Day out of range - Maximum value: 31");

        if (!IsValidDay(year, month, day)) throw new ArgumentOutOfRangeException(nameof(day), $"Day out of range - For the given year: {Year} and month: {Month}");
        
        Year = year;
        Month = month;
        Day = day;
    }

    public int Year { get; }

    public int Month { get; }

    public int Day { get; }

    //public int Age(DateOnly from)
    //{
    //    var now = (from.Year * 100 + from.Month) * 100 + from.Day;
    //    var dob = (Year * 100 + Month) * 100 + Day;
    //    int age = (now - dob) / 10000;
    //    return age;
    //}

    private int YearMinimumValue => DateTime.MinValue.Year;

    private int YearMaximumValue => DateTime.MaxValue.Year;

    private bool IsValidDay(int year, int month, int day)
    {
        if (day <= 28) return true;
        var dt = new DateTime(year, month, 1);
        if (dt.Year == DateTime.MaxValue.Year && dt.Month == DateTime.MaxValue.Month) return true;
        dt = dt.AddMonths(1).AddDays(-1);
        if (day <= dt.Day) return true;
        return false;
    }

    public override string ToString()
    {
        return $"{Year:0000}-{Month:00}-{Day:00}";
    }
}