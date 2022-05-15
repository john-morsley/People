namespace Morsley.UK.People.API.Test.Fixture;

// Responsible for:
// 1. The SUT --> The Write application
// 2. Creating a Docker MongoDB instance for the SUT
// 3. Creating a Docker RabbitMQ instance for the SUT
// 4. Creating a Docker MongoDB instance for the SUT
public class WriteApplicationTestFixture<TProgram> : SecuredApplicationTestFixture<TProgram> where TProgram : class
{
    public BusTestFixture BusTestFixture => _busTestFixture!;

    public DatabaseTestFixture ReadDatabase => _readDatabaseTestFixture!;

    public DatabaseTestFixture WriteDatabase => _writeDatabaseTestFixture!;

    public CacheTestFixture Cache => _cacheTestFixture;

    protected BusTestFixture? _busTestFixture;

    protected DatabaseTestFixture? _readDatabaseTestFixture;

    protected DatabaseTestFixture? _writeDatabaseTestFixture;

    protected CacheTestFixture? _cacheTestFixture;

    [OneTimeSetUp]
    protected async override Task OneTimeSetUp()
    {
        var configuration = GetConfiguration();

        _readDatabaseTestFixture = new DatabaseTestFixture("Read_Database_Test", configuration, "ReadMongoDBSettings");
        await _readDatabaseTestFixture.CreateDatabase();
        var readDatabaseConfiguration = _readDatabaseTestFixture.Configuration;
        configuration = UpdateConfiguration(configuration, readDatabaseConfiguration);

        _writeDatabaseTestFixture = new DatabaseTestFixture("Write_Database_Test", configuration, "WriteMongoDBSettings");
        await _writeDatabaseTestFixture.CreateDatabase();
        var writeDatabaseConfiguration = _writeDatabaseTestFixture.Configuration;
        configuration = UpdateConfiguration(configuration, writeDatabaseConfiguration);

        _cacheTestFixture = new CacheTestFixture("Cache_Test", configuration, "RedisCacheSettings");
        await _cacheTestFixture.CreateCache();
        var cacheConfiguration = _cacheTestFixture.Configuration;
        configuration = UpdateConfiguration(configuration, cacheConfiguration);

        _busTestFixture = new BusTestFixture("Bus_Test", configuration, "ReadMongoDBSettings");
        await _busTestFixture.CreateBus();
        var busConfiguration = _busTestFixture.Configuration;
        configuration = UpdateConfiguration(configuration, busConfiguration);

        _busTestFixture.Subscribe<PersonAddedEvent, PersonAddedEventHandler>();
        _busTestFixture.Subscribe<PersonDeletedEvent, PersonDeletedEventHandler>();
        _busTestFixture.Subscribe<PersonUpdatedEvent, PersonUpdatedEventHandler>();

        await base.OneTimeSetUp();
    }

    [SetUp]
    protected async override Task SetUp()
    {
        _busTestFixture?.SetUp();
        await _readDatabaseTestFixture!.SetUp();
        await _writeDatabaseTestFixture!.SetUp();

        await base.SetUp();
    }

    [TearDown]
    protected async override Task TearDown()
    {
        await _busTestFixture?.TearDown();
        await _readDatabaseTestFixture!.TearDown();
        await _writeDatabaseTestFixture!.TearDown();

        await base.TearDown();
    }

    [OneTimeTearDown]
    protected async override Task OneTimeTearDown()
    {
        await _busTestFixture!.OneTimeTearDown();
        await _readDatabaseTestFixture!.OneTimeTearDown();
        await _writeDatabaseTestFixture!.OneTimeTearDown();

        await base.OneTimeTearDown();
    }

    private IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder();

        builder.AddJsonFile("appsettings.json");

        var configuration = builder.Build();

        return configuration;
    }

    protected override Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();

        foreach (var additionalBusConfiguration in _busTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalBusConfiguration.Key, additionalBusConfiguration.Value);
        }

        foreach (var additionalDatabaseConfiguration in _readDatabaseTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalDatabaseConfiguration.Key, additionalDatabaseConfiguration.Value);
        }

        foreach (var additionalDatabaseConfiguration in _writeDatabaseTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalDatabaseConfiguration.Key, additionalDatabaseConfiguration.Value);
        }

        return additional;
    }
}
