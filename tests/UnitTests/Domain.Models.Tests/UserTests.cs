namespace Domain.Models.Tests;

public class UserTests
{
    [Test]
    public void EmptyUserToString()
    {
        var user = new User();
        var result = user.ToString();
    }

    [Test]
    public void CompleteUserToString()
    {
        var user = new User();
        user.FirstName = "John";
        user.LastName = "Morsley";
        user.Sex = Sex.Male;
        user.Gender = Gender.Cisgender;
        user.DateOfBirth = new DateTime(2000, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        var result = user.ToString();
    }
}
