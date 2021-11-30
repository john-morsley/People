


namespace Users.API.Write.Tests.v1;

public class POST : APIsTestBase<StartUp>
{
    [Test]
    public async Task Given_The_User_Does_Not_Exist___When_That_User_Is_Added___Then_That_User_Should_Exist()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users/";
        var addUserRequest = new Users.API.Models.Request.v1.AddUserRequest
        {
            FirstName = "John",
            LastName = "Morsley"
        };
        var addUserRequestJson = JsonSerializer.Serialize(addUserRequest);
        var payload = new StringContent(addUserRequestJson, System.Text.Encoding.UTF8, "application/json");
        var httpResponse = await _client.PostAsync(url, payload);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        NumberOfUsersInDatabase().Should().Be(1);
        var userResponseJson = await httpResponse.Content.ReadAsStringAsync();
        userResponseJson.Length.Should().BeGreaterThan(0);
        var userResponse = DeserializeUserResponse(userResponseJson);
        userResponse.Should().NotBeNull();        
        var actualUser = GetUserFromDatabase(userResponse.Id);
        httpResponse.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userResponse.Id}");
    }

    [Test]
    public async Task When_An_Unsupported_Method_Is_Called___Then_It_Should_Be_Rejected()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users/{Guid.NewGuid}";
        //var addUserRequest = new Users.API.Models.Request.v1.AddUserRequest()
        //var addUserRequestJson = JsonSerializer.Serialize(addUserRequest);
        var payload = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
        var httpResponse = await _client.PostAsync(url, payload);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

}
