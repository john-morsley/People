namespace Morsley.UK.People.Domain.Unit.Tests;

public class PeopleUnitTests
{
    [Test]
    public void MinimalPersonToString()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        var person = new Person(personId, "John", "Doe");

        // Act...
        var result = person.ToString();

        // Assert...
        result.Should().Be($"Id: {personId} | FirstName: John | LastName: Doe | Sex: [Null] | Gender: [Null] | DateOfBirth: [Null]");
    }

    [Test]
    public void CompletePersonToString()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        var person = new Person(personId, "John", "Doe")
        {
            Sex = Sex.Male,
            Gender = Gender.Cisgender,
            DateOfBirth = new DateTime(2000, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act...
        var result = person.ToString();

        // Assert...
        result.Should().Be($"Id: {personId} | FirstName: John | LastName: Doe | Sex: Male | Gender: Cisgender | DateOfBirth: 2000-04-01");
    }
}