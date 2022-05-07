namespace Morsley.UK.People.API.Test.Fixture;

// Responsible for:
// 1. The SUT --> The Read application
// 2. Creating a Docker MongoDB instance for the SUT
// 3. Creating a Docker Redis instance for the SUT
public class ReadApplicationTestFixture<TProgram> : SecuredApplicationTestFixture<TProgram> where TProgram : class
{
    public DatabaseTestFixture ApplicationReadDatabase => _readDatabaseTestFixture!;
    public CacheTestFixture ApplicationCache => _cacheTestFixture;

    protected DatabaseTestFixture? _readDatabaseTestFixture;
    protected CacheTestFixture? _cacheTestFixture;

    [OneTimeSetUp]
    protected async override Task OneTimeSetUp()
    {
        var configuration = GetConfiguration();

        _readDatabaseTestFixture = new DatabaseTestFixture("Read_Database_Test", configuration, "ReadMongoDBSettings");
        await _readDatabaseTestFixture.CreateDatabase();

        _cacheTestFixture = new CacheTestFixture("Cache_Test", configuration, "RedisCacheSettings");
        await _cacheTestFixture.CreateCache();

        await base.OneTimeSetUp();
    }

    [SetUp]
    protected async override Task SetUp()
    {
        await _readDatabaseTestFixture!.SetUp();
        await _cacheTestFixture!.SetUp();

        await base.SetUp();
    }

    [TearDown]
    protected async override Task TearDown()
    {
        await base.TearDown();

        await _readDatabaseTestFixture!.TearDown();
        await _cacheTestFixture!.TearDown();
    }

    [OneTimeTearDown]
    protected async override Task OneTimeTearDown()
    {
        await base.OneTimeTearDown();

        await _readDatabaseTestFixture!.OneTimeTearDown();
        await _cacheTestFixture!.OneTimeTearDown();
    }

    protected override Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();

        foreach (var additionalConfiguration in _readDatabaseTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalConfiguration.Key, additionalConfiguration.Value);
        }

        foreach (var additionalConfiguration in _cacheTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalConfiguration.Key, additionalConfiguration.Value);
        }

        return additional;
    }

    private IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder();

        builder.AddJsonFile("appsettings.json");

        //var busConfiguration = _busTestFixture!.GetInMemoryConfiguration();
        //var busDatabaseConfiguration = _busDatabaseTestFixture!.GetInMemoryConfiguration();

        var configuration = builder.Build();

        return configuration;
    }
}