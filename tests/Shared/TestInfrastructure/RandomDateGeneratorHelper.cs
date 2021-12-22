namespace TestInfrastructure;

public static class RandomDateGeneratorHelper
{
    private const int NumberOfDaysInOneHundredYears = 36525;

    public static DateOnly GenerateRandomDateOfBirth()
    {
        var random = new Random(DateTime.Now.Millisecond);
        var now = DateTime.Now;
        var days = random.Next(1, NumberOfDaysInOneHundredYears);
        var dob = now.AddDays(days * -1);
        return new DateOnly(dob.Year, dob.Month, dob.Day);
    }
}