namespace TestInfrastructure.AutoFixture;

public class DateOfBirthSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var seededRequest = request as SeededRequest;

        if (seededRequest == null) return new NoSpecimen();

        var type = seededRequest.Request as Type;

        if (type == null) return new NoSpecimen();

        if (type != typeof(DateOnly)) return new NoSpecimen();

        return GenerateRandomDateOfBirth();
    }

    private DateOnly GenerateRandomDateOfBirth()
    {
        var dob = RandomDateGeneratorHelper.GenerateRandomDateOfBirth();
        return new DateOnly(dob.Year, dob.Month, dob.Day);
    }
}
