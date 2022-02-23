namespace TestInfrastructure.AutoFixture;

public class UserSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var seededRequest = request as SeededRequest;

        if (seededRequest == null) return new NoSpecimen();

        var type = seededRequest.Request as Type;

        if (type == null) return new NoSpecimen();

        if (type != typeof(Users.Domain.Models.User)) return new NoSpecimen();

        var firstName = nameof(Users.API.Models.Request.v1.AddUserRequest.FirstName) + "___" + context.Create<string>();
        var lastName = nameof(Users.API.Models.Request.v1.AddUserRequest.LastName) + "___" + context.Create<string>();
        var dob = RandomGeneratorHelper.GenerateRandomDateOfBirth();

        var user = new Users.Domain.Models.User(firstName, lastName)
        {
            FirstName = firstName,
            LastName = lastName,
            Sex = RandomGeneratorHelper.GenerateRandomNullableSex(),
            Gender = RandomGeneratorHelper.GenerateRandomNullableGender(),
            DateOfBirth = new DateTime(dob.Year, dob.Month, dob.Day, 0, 0, 0, 0, DateTimeKind.Utc)
        };

        return user;
    }
}
