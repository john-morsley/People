namespace Users.API.Write.Tests.v1.Methods;

public class PUT : APIsTestBase<StartUp>
{
    [Test]
    public async Task Given_The_User_Does_Exist___When_That_User_Is_Upserted___Then_That_User_Should_Be_Updated()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var userToBeUpdated = GenerateTestUser(userId);
        AddUserToDatabase(userToBeUpdated);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        var url = $"/api/v1/users/{userId}";
        var upsertUserRequest = new Users.API.Models.Request.v1.UpsertUserRequest
        {
            FirstName = "John",
            LastName = "Morsley"
        };
        var addUserRequestJson = JsonSerializer.Serialize(upsertUserRequest);
        var payload = new StringContent(addUserRequestJson, System.Text.Encoding.UTF8, "application/json");
        var httpResponse = await _client.PutAsync(url, payload);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        NumberOfUsersInDatabase().Should().Be(1);
        //var userResponseJson = await httpResponse.Content.ReadAsStringAsync();
        //userResponseJson.Length.Should().BeGreaterThan(0);
        //var userResponse = DeserializeUserResponse(userResponseJson);
        //userResponse.Should().NotBeNull();
        //var actualUser = GetUserFromDatabase(userResponse.Id);
        //httpResponse.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userResponse.Id}");
    }

    [Test]
    public async Task Given_The_User_Does_Not_Exist___When_That_User_Is_Upserted___Then_That_User_Should_Be_Inserted()
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
        var httpResponse = await _client.PutAsync(url, payload);

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

}
