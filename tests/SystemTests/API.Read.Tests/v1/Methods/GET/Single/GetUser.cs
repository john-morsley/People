namespace Users.API.Read.Tests.v1.Methods.GET.Single;

public class GetUser : APIsTestBase<StartUp>
{
    [Test]
    [Category("Happy")]
    public async Task Given_User_Exists___When_That_User_Is_Requested___Then_200_OK_And_User_Returned()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var userId = Guid.NewGuid();
        var expected = GenerateTestUser(userId);
        AddUserToDatabase(expected);

        NumberOfUsersInDatabase().Should().Be(1);

        var url = $"/api/v1/users/{userId}";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
        var userData = DeserializeUserResource(content);
        userData.Should().NotBeNull();

        // - User
        userData.Data.Should().NotBeNull();
        ShouldBeEquivalentTo(userData, expected);

        // - Links
        userData.Links.Should().NotBeNull();
        userData.Links.Count.Should().Be(2);
        LinksForUserShouldBeCorrect(userData.Links, userId);

        // - Embedded
        userData.Embedded.Should().BeNull();
    }

    [Test]
    [Category("Happy")]
    public async Task Given_User_Does_Not_Exist___When_That_User_Is_Requested___Then_204_NoContent()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var userId = Guid.NewGuid();
        var url = $"/api/v1/users/{userId}";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_User_Is_Requested_With_Invalid_Id___Then_404_NotFound()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        const string url = "/api/v1/users/invalid-user-id";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }
}