namespace Users.API.Read.Tests.v1.Methods;

public class GET : APIsTestBase<StartUp>
{
    [Test]
    public async Task Given_The_User_Does_Not_Exist_In_The_Database___When_That_User_Is_Requested___Then_That_User_Should_Not_Found()
    {
        // Arrange...
        var userId = Guid.NewGuid();

        var url = $"/api/v1/users/{userId}";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().Be(0);
    }

    [Test]
    public async Task Given_The_User_Exists_In_The_Database___When_That_User_Is_Requested___Then_That_User_Should_Be_Returned()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var expected = GenerateTestUser(userId);
        AddUserToDatabase(expected);

        var url = $"/api/v1/users/{userId}";
            
        // Act...
        var httpResponse = await _client.GetAsync(url);
            
        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var actual = DeserializeUserResponse(response);
        actual.Should().NotBeNull();
        actual.Id.Should().Be(expected.Id);
        actual.FirstName.Should().Be(expected.FirstName);
        actual.LastName.Should().Be(expected.LastName);
    }

    [Test]
    public async Task Given_No_Users_Exist_In_The_Database___When_A_Page_Of_Users_Is_Requested___Then_No_Users_Should_Be_Returned()
    {
        // Arrange...
        var url = $"/api/v1/users?PageNumber=1&PageSize=10";
            
        // Act...
        var httpResponse = await _client.GetAsync(url);
            
        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().Be(0);
    }

    [Test]
    public async Task Given_One_User_Exists_In_The_Database___When_A_Page_Of_Users_Is_Requested___Then_A_Page_Of_Users_Should_Be_Returned()
    {
        // Arrange...
        var user = GenerateTestUser();
        AddUserToDatabase(user);
        var url = $"/api/v1/users?PageNumber=1&PageSize=10";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        //var pageOfUsers = JsonSerializer.Deserialize<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>(response);
        var pageOfUsers = DeserializePagedListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count.Should().Be(1);
        pageOfUsers.IsReadOnly.Should().BeFalse();
        pageOfUsers.HasPrevious.Should().BeFalse();
        pageOfUsers.HasNext.Should().BeFalse();
        pageOfUsers.CurrentPage.Should().Be(1);
        pageOfUsers.TotalPages.Should().Be(1);
        //pageOfUsers.TotalCount.Should().Be(0);
        pageOfUsers.PageSize.Should().Be(10);
    }

    [Test]
    public async Task Given_Three_Users_Exists_In_The_Database___When_A_Page_Of_Users_Is_Requested___Then_A_Page_Of_Users_Should_Be_Returned()
    {
        // Arrange...
        var user1 = GenerateTestUser();
        AddUserToDatabase(user1);
        var user2 = GenerateTestUser();
        AddUserToDatabase(user2);
        var user3 = GenerateTestUser();
        AddUserToDatabase(user3);
        var url = $"/api/v1/users?PageNumber=1&PageSize=10";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        //var pageOfUsers = JsonSerializer.Deserialize<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>(response);
        var pageOfUsers = DeserializePagedListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count.Should().Be(3);
        pageOfUsers.IsReadOnly.Should().BeFalse();
        pageOfUsers.HasPrevious.Should().BeFalse();
        pageOfUsers.HasNext.Should().BeFalse();
        pageOfUsers.CurrentPage.Should().Be(1);
        pageOfUsers.TotalPages.Should().Be(1);
        //pageOfUsers.TotalCount.Should().Be(0);
        pageOfUsers.PageSize.Should().Be(10);
    }
}
