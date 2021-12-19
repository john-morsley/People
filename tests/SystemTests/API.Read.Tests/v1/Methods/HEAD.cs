namespace Users.API.Read.Tests.v1.Methods;

public class HEAD : APIsTestBase<StartUp>
{
    [Test]
    [Category("Happy")]
    public async Task Given_The_User_Does_Not_Exist_In_The_Database___When_That_User_Is_Requested___Then_That_User_Should_Not_Found()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var url = $"/api/v1/users/{userId}";

        // Act...
        //var httpResponse = await _client.GetAsync(url);
        var httpResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        httpResponse.Content.Headers.ContentLength.Should().BeNull();
        var response = await httpResponse.Content.ReadAsStringAsync();        
        response.Length.Should().Be(0);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_The_User_Exists___When_That_User_Is_Requested___Then_200_OK_And_Content_Length_Returned()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var expected = GenerateTestUser(userId);
        AddUserToDatabase(expected);              
        var url = $"/api/v1/users/{userId}";
        var expectedContentLength = await GetExpectedContentLength(url);

        // Act...
        var httpResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        httpResponse.Content.Headers.ContentLength.Should().Be(expectedContentLength);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().Be(0);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_No_Users_Exist___When_A_Page_Of_Users_Is_Requested___Then_204_NoContent()
    {
        // Arrange...
        var url = $"/api/v1/users?PageNumber=1&PageSize=10";

        // Act...
        var httpResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        httpResponse.Content.Headers.ContentLength.Should().BeNull();
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().Be(0);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Content_Length_Returned()
    {
        // Arrange...
        var user = GenerateTestUser();
        AddUserToDatabase(user);
        var url = $"/api/v1/users?PageNumber=1&PageSize=10";
        var expectedContentLength = await GetExpectedContentLength(url);

        // Act...
        var httpResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        httpResponse.Content.Headers.ContentLength.Should().Be(expectedContentLength);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().Be(0);
    }

    private async Task<long> GetExpectedContentLength(string url)
    {
        var httpResponse = await _client.GetAsync(url);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        return response.Length;
    }

    //[Test]
    //public async Task Given_Three_Users_Exists_In_The_Database___When_A_Page_Of_Users_Is_Requested___Then_A_Page_Of_Users_Should_Be_Returned()
    //{
    //    // Arrange...
    //    var user1 = GenerateTestUser();
    //    AddUserToDatabase(user1);
    //    var user2 = GenerateTestUser();
    //    AddUserToDatabase(user2);
    //    var user3 = GenerateTestUser();
    //    AddUserToDatabase(user3);
    //    var url = $"/api/v1/users?PageNumber=1&PageSize=10";

    //    // Act...
    //    var httpResponse = await _client.GetAsync(url);

    //    // Assert...
    //    httpResponse.IsSuccessStatusCode.Should().BeTrue();
    //    httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    //    var response = await httpResponse.Content.ReadAsStringAsync();
    //    response.Length.Should().BeGreaterThan(0);
    //    //var pageOfUsers = JsonSerializer.Deserialize<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>(response);
    //    var pageOfUsers = DeserializePagedListOfUserResponses(response);
    //    pageOfUsers.Should().NotBeNull();
    //    pageOfUsers.Count.Should().Be(3);
    //    pageOfUsers.IsReadOnly.Should().BeFalse();
    //    pageOfUsers.HasPrevious.Should().BeFalse();
    //    pageOfUsers.HasNext.Should().BeFalse();
    //    pageOfUsers.CurrentPage.Should().Be(1);
    //    pageOfUsers.TotalPages.Should().Be(1);
    //    //pageOfUsers.TotalCount.Should().Be(0);
    //    pageOfUsers.PageSize.Should().Be(10);
    //}
}
