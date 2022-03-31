namespace Morsley.UK.People.API.Test.Fixture;

// Responsible for:
// 1. The SUT --> The Read application
// 2. Creating a Docker MongoDB instance for the SUT
public class ReadApplicationTestFixture<TProgram> : SecuredApplicationTestFixture<TProgram> where TProgram : class
{
    public DatabaseTestFixture ApplicationReadDatabase => _readDatabaseTestFixture!;

    protected DatabaseTestFixture? _readDatabaseTestFixture;

    public ReadApplicationTestFixture()
    {
        
    }

    [OneTimeSetUp]
    protected async override Task OneTimeSetUp()
    {
        var readDatabaseConfiguration = GetReadDatabaseConfiguration();
        _readDatabaseTestFixture = new DatabaseTestFixture("Read_Database_Test", readDatabaseConfiguration, "ReadMongoDBSettings");
        await _readDatabaseTestFixture.CreateDatabase();
    }

    [SetUp]
    protected virtual void SetUp()
    {
        _readDatabaseTestFixture!.SetUp();
    }

    [TearDown]
    protected virtual void TearDown()
    {
        _readDatabaseTestFixture?.TearDown();
    }

    [OneTimeTearDown]
    protected async virtual Task OneTimeTearDown()
    {
        await _readDatabaseTestFixture!.OneTimeTearDown();
    }

    protected override Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();

        foreach (var additionalDatabaseConfiguration in _readDatabaseTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalDatabaseConfiguration.Key, additionalDatabaseConfiguration.Value);
        }

        return additional;
    }

    private IConfiguration GetReadDatabaseConfiguration()
    {
        var builder = new ConfigurationBuilder();

        builder.AddJsonFile("appsettings.json");

        //var busConfiguration = _busTestFixture!.GetInMemoryConfiguration();
        //var busDatabaseConfiguration = _busDatabaseTestFixture!.GetInMemoryConfiguration();

        var configuration = builder.Build();

        return configuration;
    }

}
