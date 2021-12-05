namespace Users.API.Write.Tests.v1.Methods;

public class DELETE : APIsTestBase<StartUp>
{
    [Test]
    public async Task Given_User_Exists___When_Delete_Is_Attempted___Then_NoContent_And_User_Deleted()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var userToBeDeleted = GenerateTestUser(userId);
        AddUserToDatabase(userToBeDeleted);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        var url = $"/api/v1/users/{userId}";
        var httpResponse = await _client.DeleteAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);        
        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
        NumberOfUsersInDatabase().Should().Be(0);
        var shouldNotExistUser = GetUserFromDatabase(userId);
        shouldNotExistUser.Should().BeNull();
    }

    [Test]
    public async Task Given_User_Does_Not_Exist___When_Delete_Is_Attempted___Then_404_NotFound()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users/{userId}";
        var httpResponse = await _client.DeleteAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
        // ToDo --> Check the error object
    }
}
