namespace Morsley.UK.People.API.Write.Tests.Methods;

public class DELETE : WriteApplicationTestFixture<WriteProgram>
{
    [Test]
    [Category("Happy")]
    public async Task Given_User_Exists___When_Delete_Is_Attempted___Then_NoContent_And_User_Deleted()
    {
        // Arrange...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);
        var userToBeDeleted = DatabaseTestFixture.GenerateTestPerson();
        var userId = userToBeDeleted.Id;
        DatabaseTestFixture.AddPersonToDatabase(userToBeDeleted);
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/person/{userId}";
        var result = await HttpClient!.DeleteAsync(url);

        // Assert...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);        
        
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
        
        var shouldNotExistUser = DatabaseTestFixture.GetPersonFromDatabase(userId);
        shouldNotExistUser.Should().BeNull();
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_Not_Authenticated_And_Delete_Is_Attempted___Then_401_NotFound()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/person/{userId}";
        var result = await HttpClient!.DeleteAsync(url);

        // Assert...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

    [Test]
    [Category("Unhappy")]
    public async Task Given_User_Does_Not_Exist___When_Delete_Is_Attempted___Then_404_NotFound()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/person/{userId}";
        var result = await HttpClient!.DeleteAsync(url);

        // Assert...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }
}
