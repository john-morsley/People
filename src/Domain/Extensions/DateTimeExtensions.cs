namespace Users.Domain.Models;

public static class DateOnlyExtensions
{
    public static int Age(this DateOnly to, DateOnly from)
    {
        var now = (from.Year * 100 + from.Month) * 100 + from.Day;
        var dob = (to.Year * 100 + to.Month) * 100 + to.Day;
        int age = (now - dob) / 10000;
        return age;
    }
}