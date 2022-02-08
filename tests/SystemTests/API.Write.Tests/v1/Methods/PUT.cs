namespace Users.API.Write.Tests.v1.Methods;

public class PUT : APIsTestBase<StartUp>
{
    // If an attempt is made to update a user via PUT and the user exists, then the user should be updated and a 200 OK
    // HTTP status code should be returned.
    [Test]
    [Category("Happy")]
    public async Task Given_User_Exists___When_Put_Update_User___Then_200_OK_And_User_Updated()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var originalUser = GenerateTestUser(userId);
        AddUserToDatabase(originalUser);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        var url = $"/api/v1/users/{userId}";
        var updateUserRequest = GenerateTestUpdateUserRequest(originalUser.Sex, originalUser.Gender);
        var updateUserRequestJson = JsonSerializer.Serialize(updateUserRequest);
        var payload = new StringContent(updateUserRequestJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var httpResponse = await _client.PutAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userData = DeserializeUserData(content);
        var actualUser = GetUserFromDatabase(userId);
        // As an update has taken place, every property of the actual user, should differ from the original user (except Id).
        actualUser.Should().NotBeEquivalentTo(originalUser);

        actualUser.Should().BeEquivalentTo(updateUserRequest);
    }

    // If an attempt is made to update a user via PUT and the user does not exist, then the user should be created and
    // a 201 Created HTTP status code should be returned. In addition, the Location in the response Header should
    // contain a link to the newly created resource.
    [Test]
    [Category("Happy")]
    public async Task Given_User_Does_Not_Exist___When_Put_Update_User___Then_201_Created_And_User_Created()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var userId = Guid.NewGuid();
        var url = $"/api/v1/users/{userId}";
        var upsertUserRequest = GenerateTestUpdateUserRequest();
        var upsertUserRequestJson = JsonSerializer.Serialize(upsertUserRequest);
        var payload = new StringContent(upsertUserRequestJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var response = await _client.PutAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userData = DeserializeUserData(content);
        userData.Should().NotBeNull();

        // - User
        userData.User.Should().BeNull();

        // - Links
        userData.Links.Should().BeNull();

        // - Embedded
        userData.Embedded.Should().BeNull();

        //var actualUser = GetUserFromDatabase(userResponse.Id);
        //actualUser.Id.Should().Be(userId);
        //actualUser.Should().BeEquivalentTo(upsertUserRequest);
        //httpResponse.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userId}");
        //throw new NotImplementedException();
    }

    // 
    [Test]
    [Category("Unhappy")]
    public async Task When_Put_Invalid_Data___Then_400_BadRequest_And_Errors_Object_Should_Detail_Issues()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var userId = Guid.NewGuid();
        var url = $"/api/v1/users/{userId}";
        var addUserRequestJson = JsonSerializer.Serialize("");
        var payload = new StringContent(addUserRequestJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var httpResponse = await _client.PutAsync(url, payload);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await httpResponse.Content.ReadAsStringAsync();
        // ToDo --> Validate error object

        //userResponseJson.Length.Should().BeGreaterThan(0);
        //var userResponse = DeserializeUser(userResponseJson);
        //userResponse.Should().NotBeNull();
        //var actualUser = GetUserFromDatabase(userResponse.Id);
        //httpResponse.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userResponse.Id}");
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_Put_Invalid_Update_User___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var userId = Guid.NewGuid();
        var url = $"/api/v1/users/{userId}";
        var updateUserRequest = GenerateTestUpdateUserRequest();
        updateUserRequest.FirstName = string.Empty;
        updateUserRequest.LastName = string.Empty;
        var updateUserRequestJson = JsonSerializer.Serialize(updateUserRequest);
        var payload = new StringContent(updateUserRequestJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var httpResponse = await _client.PutAsync(url, payload);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var content = await httpResponse.Content.ReadAsStringAsync();
        // ToDo --> Validate error object

        //userResponseJson.Length.Should().BeGreaterThan(0);
        //var userResponse = DeserializeUser(userResponseJson);
        //userResponse.Should().NotBeNull();
        //var actualUser = GetUserFromDatabase(userResponse.Id);
        //httpResponse.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userResponse.Id}");
    }
}
