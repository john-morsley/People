namespace Morsley.UK.People.Test.Fixture;

public class CacheTestFixture
{
    protected DockerTestFixture<RedisInDocker>? DockerTestFixture;

    protected IConfiguration? _configuration;
    protected readonly string _cacheKey;
    private string _name;

    public IConfiguration? Configuration => _configuration;

    protected int ContainerPort
    {
        get
        {
            if (DockerTestFixture == null) throw new InvalidOperationException("Docker has not yet been initialised!");
            return DockerTestFixture!.GetContainerPort();
        }
    }

    private ConnectionMultiplexer? _redis;

    private ConnectionMultiplexer? Redis
    {
        get
        {
            if (_redis is null)
            {
                var redisInDocker = DockerTestFixture?.InDocker as RedisInDocker;
                if (redisInDocker is null) throw new InvalidOperationException("Did not expect DockerTestFixture.InDocker as RedisInDocker to be null!");
                _redis = redisInDocker.Redis;
            }
            return _redis;
        }
    }

    public CacheTestFixture(string name, string cacheKey)
    {
        if (name == null) throw new ArgumentNullException("name");
        if (name.Length == 0) throw new ArgumentOutOfRangeException("name");

        //AutoFixture = new global::AutoFixture.Fixture();
        //AutoFixture.Customizations.Add(new DateOfBirthSpecimenBuilder());
        ////AutoFixture.Customizations.Add(new AddPersonRequestSpecimenBuilder());
        //AutoFixture.Customizations.Add(new PersonSpecimenBuilder());

        _name = name;
        _cacheKey = cacheKey;
    }

    public CacheTestFixture(string name, IConfiguration configuration, string cacheKey)
    {
        if (name == null) throw new ArgumentNullException("name");
        if (name.Length == 0) throw new ArgumentOutOfRangeException("name");
        if (configuration == null) throw new ArgumentNullException("configuration");

        //AutoFixture = new global::AutoFixture.Fixture();
        //AutoFixture.Customizations.Add(new DateOfBirthSpecimenBuilder());
        ////AutoFixture.Customizations.Add(new AddPersonRequestSpecimenBuilder());
        //AutoFixture.Customizations.Add(new PersonSpecimenBuilder());

        _name = name;
        _configuration = configuration;
        _cacheKey = cacheKey;
    }

    [OneTimeSetUp]
    public async Task CreateCache()
    {
        var potentialPort = _configuration[$"{_cacheKey}:Port"];
        //var username = _configuration[$"{_cacheKey}:Username"];
        //var password = _configuration[$"{_cacheKey}:Password"];

        if (!int.TryParse(potentialPort, out var port))
        {
            throw new NotImplementedException("Port was not a number!");
        }

        DockerTestFixture = new DockerTestFixture<RedisInDocker>(_name, port);

        try
        {
            await DockerTestFixture.RunBeforeTests();
        }
        catch (Exception e)
        {
            throw new Exception("This may be Docker related. Check Docker is running.", e);
        }

        UpdateConfiguration();

        if (_configuration == null) Assert.Fail("Configuration should not be null!");
    }

    [SetUp]
    public async virtual Task SetUp()
    {
        await DeleteAllFromCacheAsync();
    }

    [TearDown]
    public async virtual Task TearDown()
    {
        await DeleteAllFromCacheAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await DockerTestFixture!.RunAfterTests();
    }

    private async Task DeleteAllFromCacheAsync()
    {
        var server = Redis?.GetServer("localhost", ContainerPort);
        if (server is null) throw new InvalidOperationException("Did not expect server to be null!");
        await server!.FlushAllDatabasesAsync();
    }

    public Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();
        if (DockerTestFixture != null) additional.Add($"{_cacheKey}:Port", ContainerPort.ToString());
        return additional;
    }

    private void UpdateConfiguration()
    {
        var builder = new ConfigurationBuilder().AddConfiguration(_configuration);
        builder.AddInMemoryCollection(GetInMemoryConfiguration());
        var configuration = builder.Build();
        var potentialPort = configuration[$"{_cacheKey}:Port"];
        if (!int.TryParse(potentialPort, out var port))
        {
            throw new NotImplementedException("Port was not a number!");
        }
        if (port != DockerTestFixture!.InDocker.Port)
        {
            throw new NotImplementedException("Port has not been updated!");
        }
        _configuration = configuration;
    }
}