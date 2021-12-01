namespace Users.API.Write.Tests.v1.Methods;

public class DELETE : APIsTestBase<StartUp>
{
    [Test]
    public async Task Given_A_User_Exists___When_That_User_Is_Deleted___Then_That_User_Should_No_Longer_Exist()
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
}
