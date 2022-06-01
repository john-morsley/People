namespace Morsley.UK.People.API.Test.Fixture;

// Responsible for:
// 1. The SUT --> The Write application
// 2. Creating a Docker MongoDB instance for the SUT
// 3. Creating a Docker RabbitMQ instance for the SUT
// 4. Creating a Docker MongoDB instance for the SUT
public class WriteApplicationTestFixture<TProgram> : SecuredApplicationTestFixture<TProgram> where TProgram : class
{
    public BusTestFixture BusTestFixture => _busTestFixture!;

    public CacheTestFixture Cache => _cacheTestFixture;

    public DatabaseTestFixture ReadDatabase => _readDatabaseTestFixture!;

    public DatabaseTestFixture WriteDatabase => _writeDatabaseTestFixture!;
   
    protected BusTestFixture? _busTestFixture;

    protected CacheTestFixture? _cacheTestFixture;

    protected DatabaseTestFixture? _readDatabaseTestFixture;

    protected DatabaseTestFixture? _writeDatabaseTestFixture;       

    [OneTimeSetUp]
    protected async override Task OneTimeSetUp()
    {
        var initial = GetConfiguration();

        var configurator = new Configurator();
        configurator.AddConfiguration(initial);
        _configuration = configurator.Build();

        _readDatabaseTestFixture = new DatabaseTestFixture("Read_Database_Test", _configuration, "ReadMongoDBSettings");
        await _readDatabaseTestFixture.CreateDatabase();
        var readDatabaseConfiguration = _readDatabaseTestFixture.Configuration;

        configurator.AddConfiguration(readDatabaseConfiguration);
        _configuration = configurator.Build();
        
        _writeDatabaseTestFixture = new DatabaseTestFixture("Write_Database_Test", _configuration, "WriteMongoDBSettings");
        await _writeDatabaseTestFixture.CreateDatabase();
        var writeDatabaseConfiguration = _writeDatabaseTestFixture.Configuration;

        configurator.AddConfiguration(writeDatabaseConfiguration);
        _configuration = configurator.Build();

        _cacheTestFixture = new CacheTestFixture("Cache_Test", _configuration, "RedisCacheSettings");
        await _cacheTestFixture.CreateCache();
        var cacheConfiguration = _cacheTestFixture.Configuration;

        configurator.AddConfiguration(cacheConfiguration);
        _configuration = configurator.Build();

        _busTestFixture = new BusTestFixture("Bus_Test", _configuration, "RabbitMQSettings", "ReadMongoDBSettings");
        await _busTestFixture.CreateBus();
        var busConfiguration = _busTestFixture.Configuration;
        
        configurator.AddConfiguration(busConfiguration);
        _configuration = configurator.Build();

        _busTestFixture.Subscribe<PersonAddedEvent, PersonAddedEventHandler>();
        _busTestFixture.Subscribe<PersonDeletedEvent, PersonDeletedEventHandler>();
        _busTestFixture.Subscribe<PersonUpdatedEvent, PersonUpdatedEventHandler>();

        await base.OneTimeSetUp();
    }

    //private IConfiguration AddToConfiguration(IConfiguration configuration)
    //{
    //    var cb = new ConfigurationBuilder();

    //    var cs = new MemoryConfigurationSource();

    //    var data = new List<KeyValuePair<string, string>>();

    //    foreach (var child in configuration.GetChildren())
    //    {
    //        var key = child.Key;
    //        var kvp = GetKeyAndValue(child, key);
    //    }

    //    //var item = new KeyValuePair<string, string>("a", "b");

    //    //data.Add(item);

    //    cs.InitialData = data;

    //    cb.Add(cs);

    //    var c = cb.Build();

    //    return c;
    //}

    //private KeyValuePair<string, object> GetKeyAndValue(IConfigurationSection? section, string key)
    //{
    //    var value = section.Value;
    //    var children = section.GetChildren();
    //    if (!children.Any()) return new KeyValuePair<string, object>(key, value);

    //    var kvp = new KeyValuePair<string, object>();
    //    foreach (var child in children)
    //    {
    //        key = key + ":" + child.Key;
    //        value = child.Value;
    //        //var inner = child.GetChildren();
    //        kvp = GetKeyAndValue(child, key);
            

    //    }

    //    return kvp;
    //} 

    [SetUp]
    protected async override Task SetUp()
    {
        await _busTestFixture?.SetUp();
        await _cacheTestFixture!.SetUp();
        await _readDatabaseTestFixture!.SetUp();
        await _writeDatabaseTestFixture!.SetUp();
        
        await base.SetUp();
    }

    [TearDown]
    protected async override Task TearDown()
    {
        await _busTestFixture?.TearDown();
        await _cacheTestFixture!.TearDown();
        await _readDatabaseTestFixture!.TearDown();
        await _writeDatabaseTestFixture!.TearDown();

        await base.TearDown();
    }

    [OneTimeTearDown]
    protected async override Task OneTimeTearDown()
    {
        await _busTestFixture!.OneTimeTearDown();
        await _cacheTestFixture!.OneTimeTearDown();
        await _readDatabaseTestFixture!.OneTimeTearDown();
        await _writeDatabaseTestFixture!.OneTimeTearDown();

        await base.OneTimeTearDown();
    }

    //private IConfiguration GetConfiguration()
    //{
    //    var builder = new ConfigurationBuilder();

    //    builder.AddJsonFile("appsettings.json");

    //    var configuration = builder.Build();

    //    return configuration;
    //}

    protected override Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();

        if (_busTestFixture is not null)
        {
            foreach (var additionalBusConfiguration in _busTestFixture!.GetInMemoryConfiguration())
            {
                additional.Add(additionalBusConfiguration.Key, additionalBusConfiguration.Value);
            }
        }

        if (_cacheTestFixture is not null)
        {
            foreach (var additionalCacheConfiguration in _cacheTestFixture!.GetInMemoryConfiguration())
            {
                additional.Add(additionalCacheConfiguration.Key, additionalCacheConfiguration.Value);
            }
        }

        if (_readDatabaseTestFixture is not null)
        {
            foreach (var additionalDatabaseConfiguration in _readDatabaseTestFixture!.GetInMemoryConfiguration())
            {
                additional.Add(additionalDatabaseConfiguration.Key, additionalDatabaseConfiguration.Value);
            }
        }

        if (_writeDatabaseTestFixture is not null)
        {
            foreach (var additionalDatabaseConfiguration in _writeDatabaseTestFixture!.GetInMemoryConfiguration())
            {
                additional.Add(additionalDatabaseConfiguration.Key, additionalDatabaseConfiguration.Value);
            }
        }

        return additional;
    }
}
