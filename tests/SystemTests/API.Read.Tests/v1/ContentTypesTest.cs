namespace Users.API.Read.Tests.v1;

public class ContentTypesTest : APIsTestBase<StartUp>
{
    [Test]
    //Attempting_To_Request_An_Unsupported_Media_Type_Should_Result_In_A_Not_Acceptable_Status_Code
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
