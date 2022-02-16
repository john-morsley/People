using System.Collections.Generic;

namespace Users.API.Read.Tests.v1.Methods;

public class OPTIONS
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
    [Category("Happy")]
    public async Task Given_An_API_With_Unknown_Capabilities___When_An_Options_Call_Is_Made___Then_200_OK_And_Allowed_HTTP_Methods_Are_Returned()
    {
        // Arrange...
        var expectedOptions = new List<string> { "HEAD", "GET", "OPTIONS" };
        const string url = "/api/v1/users/";

        // Act...
        var message = new HttpRequestMessage(HttpMethod.Options, url);
        var result = await _client.SendAsync(message);

        // Assert...
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Content.Headers.Allow.Count.Should().Be(3);
        result.Content.Headers.Allow.Should().BeEquivalentTo(expectedOptions);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }
}