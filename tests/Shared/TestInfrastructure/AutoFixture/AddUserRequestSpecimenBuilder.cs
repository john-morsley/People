namespace TestInfrastructure.AutoFixture;

public class DateOfBirthSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var seededRequest = request as SeededRequest;

        if (seededRequest == null) return new NoSpecimen();

        var type = seededRequest.Request as Type;

        if (type == null) return new NoSpecimen();

        if (type != typeof(Users.Domain.Models.DateOfBirth)) return new NoSpecimen();

        return GenerateRandomDateOfBirth();
    }

    private Users.Domain.Models.DateOfBirth GenerateRandomDateOfBirth()
    {
        var dob = RandomDateGeneratorHelper.GenerateRandomDateOfBirth();
        return new Users.Domain.Models.DateOfBirth(dob.Year, dob.Month, dob.Day);
    }
}
