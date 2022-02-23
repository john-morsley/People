namespace TestInfrastructure.AutoFixture;

public class AddUserRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var seededRequest = request as SeededRequest;

        if (seededRequest == null) return new NoSpecimen();

        var type = seededRequest.Request as Type;

        if (type == null) return new NoSpecimen();

        if (type != typeof(Users.API.Models.Request.v1.AddUserRequest)) return new NoSpecimen();

        var firstName = nameof(Users.API.Models.Request.v1.AddUserRequest.FirstName) + "___" + context.Create<string>();
        var lastName = nameof(Users.API.Models.Request.v1.AddUserRequest.LastName) + "___" + context.Create<string>();

        var dob = RandomGeneratorHelper.GenerateRandomDateOfBirth();

        var user = new Users.API.Models.Request.v1.AddUserRequest()
        {
            FirstName = firstName,
            LastName = lastName,
            Sex = context.Create<Sex?>(),
            Gender = context.Create<Gender?>(),
            DateOfBirth = $"{dob.Year:0000}-{dob.Month:00}-{dob.Day:00}"
        };

        return user;
    }
}
