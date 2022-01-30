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

        // Act...
        var url = $"/api/v1/users/{userId}";
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        // User...
        var actual = DeserializeUser(content);
        actual.Should().NotBeNull();
        actual.Id.Should().Be(expected.Id);
        actual.FirstName.Should().Be(expected.FirstName);
        actual.LastName.Should().Be(expected.LastName);

        // Metadata Links...
        var metadata = DeserializeMetadata(content);
        metadata.Links.Count().Should().Be(2);

        var getUserLink = metadata.Links.Single(_ => _.Method == "GET");
        getUserLink.Should().NotBeNull();
        getUserLink.Method.Should().Be("GET");
        getUserLink.Relationship.Should().Be("self");
        getUserLink.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");

        var deleteUserLink = metadata.Links.Single(_ => _.Method == "DELETE");
        deleteUserLink.Should().NotBeNull();
        deleteUserLink.Method.Should().Be("DELETE");
        deleteUserLink.Relationship.Should().Be("self");
        deleteUserLink.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");
    }

    [Test]
    [Category("Happy")]
    public async Task Given_User_Does_Not_Exist___When_That_User_Is_Requested___Then_204_NoContent()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var userId = Guid.NewGuid();
        var url = $"/api/v1/users/{userId}";
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
        var userId = Guid.Empty;
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users/invalid-user-id";
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }
}
