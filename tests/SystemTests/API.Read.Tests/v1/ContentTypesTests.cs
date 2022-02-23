namespace Users.API.Read.Tests.v1;

public class ContentTypesTests : APIsTestBase<StartUp>
{
    [Test]
    [Category("Unhappy")]
    public async Task Given_Only_Certain_Media_Types_Are_Supported___When_Making_A_Request_For_An_Unsupported_Type___Then_The_Status_Code_Should_Be_Not_Acceptable()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var user = GenerateTestUser();
        AddUserToDatabase(user);

        NumberOfUsersInDatabase().Should().Be(1);

        var url = $"/api/v1/users/{user.Id}";

        _client!.DefaultRequestHeaders.Clear();
        _client!.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotAcceptable);
    }
}
