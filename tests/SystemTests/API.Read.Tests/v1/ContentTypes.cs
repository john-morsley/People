namespace Users.API.Read.Tests.v1;

public class ContentTypes : APIsTestBase<StartUp>
{
    [Test]
    public async Task Attempting_To_Request_An_Unsupported_Media_Type_Should_Result_In_A_Not_Acceptable_Status_Code()
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
