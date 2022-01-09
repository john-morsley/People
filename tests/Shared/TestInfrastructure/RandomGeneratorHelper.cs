namespace TestInfrastructure;

public static class RandomGeneratorHelper
{
    private const int NumberOfDaysInOneHundredYears = 36525;

    public static DateOnly GenerateRandomDateOfBirth()
    {
        var random = new Random();
        var now = DateTime.UtcNow;
        var days = random.Next(1, NumberOfDaysInOneHundredYears);
        var dob = now.AddDays(days * -1);
        return new DateOnly(dob.Year, dob.Month, dob.Day);
    }

    public static Sex? GenerateRandomNullableSex()
    {
        var random = new Random();
        var sex = random.Next(1, 5);
        if (sex == 1) return null;
        return (Sex)(sex - 1);
    }

    public static Gender? GenerateRandomNullableGender()
    {
        var random = new Random();
        var gender = random.Next(1, 8);
        if (gender == 1) return null;
        return (Gender)(gender - 1);
    }
}