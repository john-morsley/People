namespace Users.API.Read.Tests.v1;

public class ContentTypesTests : APIsTestBase<StartUp>
{
    [Test]
    [Category("Unhappy")]
    public async Task Given_Only_Certain_Media_Types_Are_Supported___When_Making_A_Request_For_An_Unsupported_Type___Then_The_Status_Code_Should_Be_Not_Acceptable()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var user = GenerateTestUser(userId);
        AddUserToDatabase(user);

        var url = $"/api/v1/users/{userId}";

        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        // Act...
        var httpResponse = await _client.GetAsync(url);
            
        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotAcceptable);
    }
}
