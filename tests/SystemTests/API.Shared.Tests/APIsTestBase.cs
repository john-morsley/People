namespace API.Shared.Tests;

public class APIsTestBase<TStartUp> : TestBase where TStartUp : class
{
    protected MongoContext _mongoContext;

    private  TestServer _server;
    protected HttpClient _client;

    [SetUp]
    public void Setup()
    {
        var configuration = GetCurrentConfiguration();
        _mongoContext = new MongoContext(configuration);
        _mongoContext.IsHealthy().Should().BeTrue();

        NumberOfUsersInDatabase().Should().Be(0);

        var webHostBuilder = new WebHostBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json");
                builder.AddInMemoryCollection(GetInMemoryConfiguration());
            })
            .UseEnvironment("Development")
            .UseStartup<TStartUp>();

        _server = new TestServer(webHostBuilder);
        _client = _server.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
