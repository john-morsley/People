namespace Users.Domain.Models;

public class DateOfBirth
{
    private int _year;
    private int _month;
    private int _day;

    public DateOfBirth(int year, int month, int day)
    {

    }

    public int Year { get; }

    public int Month { get; }

    public int Day { get; }

    public int Age(DateTime today)
    {
        return 0;
    }
}