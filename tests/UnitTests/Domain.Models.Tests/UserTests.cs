namespace Domain.Models.Tests;

public class UserTests
{
    [Test]
    public void EmptyUserToString()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var user = new User()
        {
            Id = userId,
        };

        // Act...
        var result = user.ToString();

        // Assert...
        result.Should().Be($"Id: {userId} | FirstName: [Null] | LastName: [Null] | Sex: [Null] | Gender: [Null] | DateOfBirth: [Null]");
    }

    [Test]
    public void CompleteUserToString()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Sex = Sex.Male,
            Gender = Gender.Cisgender,
            DateOfBirth = new DateTime(2000, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act...
        var result = user.ToString();

        // Assert...
        result.Should().Be($"Id: {userId} | FirstName: John | LastName: Doe | Sex: Male | Gender: Cisgender | DateOfBirth: 2000-04-01");
    }
}
