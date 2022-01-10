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
        var result = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

        // Assert...
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Content.Headers.ContentLength.Should().BeNull();
        var content = await result.Content.ReadAsStringAsync();        
        content.Length.Should().Be(0);
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
        var result = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

        // Assert...
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Content.Headers.ContentLength.Should().Be(expectedContentLength);
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_No_Users_Exist___When_A_Page_Of_Users_Is_Requested___Then_204_NoContent()
    {
        // Arrange...
        var url = $"/api/v1/users?PageNumber=1&PageSize=10";

        // Act...
        var result = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

        // Assert...
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Content.Headers.ContentLength.Should().BeNull();
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
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
        var result = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

        // Assert...
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Content.Headers.ContentLength.Should().Be(expectedContentLength);
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

    private async Task<long> GetExpectedContentLength(string url)
    {
        var result = await _client.GetAsync(url);
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadAsStringAsync();
        return content.Length;
    }
}
