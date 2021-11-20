namespace API.Shared.Tests;

public class APIsTestBase<TStartUp> : TestBase where TStartUp : class
{
    protected MongoContext _mongoContext;

    private  TestServer _server;
    protected HttpClient _client;

    [SetUp]
    public void Setup()
    {
        var webHostBuilder = new WebHostBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json");
            })
            .UseEnvironment("Development")
            .UseStartup<TStartUp>();

        _server = new TestServer(webHostBuilder);
        _client = _server.CreateClient();

        var configuration = GetCurrentConfiguration();
        _mongoContext = new MongoContext(configuration);
        _mongoContext.IsHealthy().Should().BeTrue();

        NumberOfUsersInDatabase().Should().Be(0);
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
