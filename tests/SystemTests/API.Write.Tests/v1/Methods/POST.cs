namespace Users.API.Write.Tests.v1.Methods;

public class POST : APIsTestBase<StartUp>
{
    [Test]
    public async Task Given_User_Does_Not_Exist___When_Post_Add_User___Then_200_OK_And_User_Should_Be_Added()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users/";
        var addUserRequest = GenerateTestAddUserRequest();
        var addUserRequestJson = JsonSerializer.Serialize(addUserRequest);
        var payload = new StringContent(addUserRequestJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var httpResponse = await _client.PostAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);        
        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
        var userResponse = DeserializeUserResponse(content);
        userResponse.Should().NotBeNull();        
        var actualUser = GetUserFromDatabase(userResponse.Id);
        httpResponse.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userResponse.Id}");
    }

    [Test]
    public async Task When_Post_Invalid_Data___Then_400_BadRequest_And_Errors_Object_Should_Detail_Issues()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users/";
        var addUserRequestJson = JsonSerializer.Serialize("");
        var payload = new StringContent(addUserRequestJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var httpResponse = await _client.PostAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await httpResponse.Content.ReadAsStringAsync();
        // ToDo --> Validate error object

        //userResponseJson.Length.Should().BeGreaterThan(0);
        //var userResponse = DeserializeUserResponse(userResponseJson);
        //userResponse.Should().NotBeNull();
        //var actualUser = GetUserFromDatabase(userResponse.Id);
        //httpResponse.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userResponse.Id}");
    }

    [Test]
    public async Task When_Post_Invalid_Add_User___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users/";
        var addUserRequest = new Users.API.Models.Request.v1.AddUserRequest
        {
            FirstName = "",
            LastName = ""
        };
        var addUserRequestJson = JsonSerializer.Serialize(addUserRequest);
        var payload = new StringContent(addUserRequestJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var httpResponse = await _client.PostAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        
        var content = await httpResponse.Content.ReadAsStringAsync();
        // ToDo --> Validate error object
        
        //userResponseJson.Length.Should().BeGreaterThan(0);
        //var userResponse = DeserializeUserResponse(userResponseJson);
        //userResponse.Should().NotBeNull();
        //var actualUser = GetUserFromDatabase(userResponse.Id);
        //httpResponse.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userResponse.Id}");
    }

    [Test]
    public async Task When_An_Unsupported_Method_Is_Called___Then_405_MethoNotAllowed()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users/{Guid.NewGuid}"; // POST with a user ID not allowed!
        var payload = new StringContent(string.Empty, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var httpResponse = await _client.PostAsync(url, payload);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }
}
