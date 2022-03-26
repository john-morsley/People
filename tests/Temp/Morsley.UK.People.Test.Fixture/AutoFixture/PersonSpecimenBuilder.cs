namespace Morsley.UK.People.Test.Fixture.AutoFixture;

public class PersonSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var seededRequest = request as SeededRequest;

        if (seededRequest == null) return new NoSpecimen();

        var type = seededRequest.Request as Type;

        if (type == null) return new NoSpecimen();

        if (type != typeof(Person)) return new NoSpecimen();

        var firstName = nameof(AddPersonRequest.FirstName) + "___" + context.Create<string>();
        var lastName = nameof(AddPersonRequest.LastName) + "___" + context.Create<string>();
        var dob = RandomGeneratorHelper.GenerateRandomDateOfBirth();

        var person = new Person(firstName, lastName)
        {
            FirstName = firstName,
            LastName = lastName,
            Sex = RandomGeneratorHelper.GenerateRandomNullableSex(),
            Gender = RandomGeneratorHelper.GenerateRandomNullableGender(),
            DateOfBirth = new DateTime(dob.Year, dob.Month, dob.Day, 0, 0, 0, 0, DateTimeKind.Utc)
        };

        return person;
    }
}