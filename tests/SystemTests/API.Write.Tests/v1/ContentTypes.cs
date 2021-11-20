namespace Users.API.Write.Tests.v1;

public class Tests
{
    private TestServer _server;
    private HttpClient _client;
        
    [SetUp]
    public void Setup()
    {
        var webHostBuilder = new WebHostBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json");
            })
            .UseEnvironment("Development")
            .UseStartup<StartUp>();
        _server = new TestServer(webHostBuilder);
        _client = _server.CreateClient();
    }

    [Test]
    public async Task Attempting_To_Request_An_Unsupported_Media_Type_Should_Result_In_A_Not_Acceptable_Status_Code()
    {
        // Arrange...
        //var userId = Guid.NewGuid();
        //var url = $"/api/v1/users/{userId}";

        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        // Act...
        //var httpResponse = await _client.GetAsync(url);

        // Assert...
        //httpResponse.IsSuccessStatusCode.Should().BeFalse();
        //httpResponse.StatusCode.Should().Be(HttpStatusCode.NotAcceptable);
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
