namespace Users.API.Read.Tests.v1.Methods;

public class GET : APIsTestBase<StartUp>
{
    [Test]
    public async Task GetUser()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var user = GenerateTestUser(userId);
        AddUserToDatabase(user);

        var url = $"/api/v1/users/{userId}";
            
        // Act...
        var httpResponse = await _client.GetAsync(url);
            
        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task GetUsers()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var url = $"/api/v1/users/";
        var getUsersRequest = new Users.API.Models.Request.v1.GetPageOfUsersRequest();
            
        // Act...
        var httpResponse = await _client.GetAsync(url);
            
        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var users = JsonConvert.DeserializeObject<IEnumerable<Users.API.Models.Response.v1.UserResponse>>(response);
        users.Count().Should().Be(1);
    }
}
