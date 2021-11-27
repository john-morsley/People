namespace Users.API.Read.Tests.v1;

public class ExceptionHandler
{
    private TestServer _server;
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        var webHostBuilder = new WebHostBuilder()
            .UseEnvironment("Production")
            .UseStartup<StartUp>();
        _server = new TestServer(webHostBuilder);
        _client = _server.CreateClient();
    }

    /*
     * This was the only way I could think of to get the API to throw an internal Server Error.
     */
    [Test]
    public async Task Given_The_API_Is_In_Production_Mode___When_It_Is_Not_Passed_Vital_Configuration___Then_An_Error_Code_And_Error_Message_Is_Returned()
    {
        // Arrange...
        var userId = Guid.NewGuid();

        var url = $"/api/v1/users/{userId}";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        httpResponse.ReasonPhrase.Should().Be("Oops, I didn't expect that to happen! :-(");
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().Be(0);
    }


    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
