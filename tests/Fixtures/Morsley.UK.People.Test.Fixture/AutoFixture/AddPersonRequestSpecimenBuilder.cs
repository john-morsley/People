namespace Morsley.UK.People.Test.Fixture.AutoFixture;

public class AddPersonRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var seededRequest = request as SeededRequest;

        if (seededRequest == null) return new NoSpecimen();

        var type = seededRequest.Request as Type;

        if (type == null) return new NoSpecimen();

        if (type != typeof(AddPersonRequest)) return new NoSpecimen();

        var firstName = nameof(AddPersonRequest.FirstName) + "___" + context.Create<string>();
        var lastName = nameof(AddPersonRequest.LastName) + "___" + context.Create<string>();

        var dob = RandomGeneratorHelper.GenerateRandomDateOfBirth();

        var person = new AddPersonRequest()
        {
            FirstName = firstName,
            LastName = lastName,
            Sex = context.Create<Sex?>(),
            Gender = context.Create<Gender?>(),
            DateOfBirth = $"{dob.Year:0000}-{dob.Month:00}-{dob.Day:00}"
        };

        return person;
    }
}