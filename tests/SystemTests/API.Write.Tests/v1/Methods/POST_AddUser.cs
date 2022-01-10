namespace Users.API.Write.Tests.v1.Methods;

public class POST_AddUser : APIsTestBase<StartUp>
{
    [Test]
    [Category("Happy")]
    public async Task Given_User_Does_Not_Exist___When_Post_Add_User___Then_200_OK_And_User_Should_Be_Added()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users/";
        var addUserRequest = GenerateTestAddUserRequest();
        var addUserRequestJson = JsonSerializer.Serialize(addUserRequest);
        var payload = new StringContent(addUserRequestJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var result = await _client.PostAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);        
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
        var userResponse = DeserializeUserResponse(content);
        userResponse.Should().NotBeNull();        
        var actualUser = GetUserFromDatabase(userResponse.Id);
        result.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userResponse.Id}");
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_Post_Invalid_Data___Then_400_BadRequest_And_Errors_Object_Should_Detail_Issues()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users/";
        var addUserRequestJson = JsonSerializer.Serialize("");
        var payload = new StringContent(addUserRequestJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var result = await _client.PostAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await result.Content.ReadAsStringAsync();
        // ToDo --> Validate error object

        //userResponseJson.Length.Should().BeGreaterThan(0);
        //var userResponse = DeserializeUserResponse(userResponseJson);
        //userResponse.Should().NotBeNull();
        //var actualUser = GetUserFromDatabase(userResponse.Id);
        //httpResponse.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userResponse.Id}");
    }

    [Test]
    [Category("Unhappy")]
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
        var result = await _client.PostAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        
        var content = await result.Content.ReadAsStringAsync();

        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        problemDetails.Title.Should().Be("Validation error(s) occurred!");
        problemDetails.Detail.Should().Be("See the errors field for details.");
        problemDetails.Instance.Should().Be("/api/v1/users/");
        problemDetails.Extensions.Should().NotBeNull();
        var traceId = problemDetails.Extensions.Where(_ => _.Key == "traceId").FirstOrDefault();
        traceId.Should().NotBeNull();
        problemDetails.Errors.Count().Should().Be(2);
        var firstNameError = problemDetails.Errors.Where(_ => _.Key == "FirstName").Single();
        firstNameError.Should().NotBeNull();
        var firstNameErrorValue = firstNameError.Value.First();
        firstNameErrorValue.Should().Be("First name cannot be empty.");
        var lastNameError = problemDetails.Errors.Where(_ => _.Key == "LastName").Single();
        lastNameError.Should().NotBeNull();
        var lastNameErrorValue = lastNameError.Value.First();
        lastNameErrorValue.Should().Be("Last name cannot be empty.");
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_An_Unsupported_Method_Is_Called___Then_405_MethoNotAllowed()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users/{Guid.NewGuid}"; // POST with a user ID not allowed!
        var payload = new StringContent(string.Empty, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var result = await _client.PostAsync(url, payload);

        // Assert...
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }
}
