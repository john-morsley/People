namespace Users.API.Read.Tests.v1.Methods;

public class GET : APIsTestBase<StartUp>
{
    [Test]
    [Category("Happy")]
    public async Task Given_User_Does_Not_Exist___When_That_User_Is_Requested___Then_204_NoContent()
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
    [Category("Happy")]
    public async Task Given_User_Exists___When_That_User_Is_Requested___Then_200_OK_And_User_Returned()
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
    [Category("Happy")]
    public async Task Given_No_Users_Exist___When_Page_Of_Users_Is_Requested___Then_204_NoContent()
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
    [Category("Happy")]
    public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_One_User_Returned()
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

        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(1);
        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);
        var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be(1);
        pagination.PageSize.Should().Be(10);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Multiple_Users_Exist___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_Users_Returned()
    {
        // Arrange...
        AddUsersToDatabase(15);
        var url = $"/api/v1/users?PageNumber=1&PageSize=10";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(10);
        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);
        var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().Be("http://localhost/api/v1/users?pageNumber=2&pageSize=10");
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(2);
        pagination.TotalCount.Should().Be(15);
        pagination.PageSize.Should().Be(10);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Multiple_Users_Exist___When_A_Page_Of_Users_Is_Requested_Without_Parameters___Then_200_OK_And_Page_Of_Users_Returned_Using_Default_Values()
    {
        // Arrange...
        AddUsersToDatabase(5);
        var url = $"/api/v1/users";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(5);
        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);
        var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be(5);
        pagination.PageSize.Should().Be(10);
    }
}
