namespace Morsley.UK.People.API.Test.Fixture;

// Responsible for:
// 1. The SUT --> The Write application
// 2. Creating a Docker MongoDB instance for the SUT
// 3. Creating a Docker RabbitMQ instance for the SUT
// 4. Creating a Docker MongoDB instance for the SUT
public class WriteApplicationTestFixture<TProgram> : SecuredApplicationTestFixture<TProgram> where TProgram : class
{
    public BusTestFixture BusTestFixture => _busTestFixture!;

    public DatabaseTestFixture ApplicationDatabase => _applicationDatabaseTestFixture!;

    protected BusTestFixture? _busTestFixture;

    protected DatabaseTestFixture? _applicationDatabaseTestFixture;

    protected DatabaseTestFixture? _busDatabaseTestFixture;

    public WriteApplicationTestFixture()
    {

    }

    [OneTimeSetUp]
    protected async override Task OneTimeSetUp()
    {
        _busTestFixture = new BusTestFixture("Bus_Test");
        await _busTestFixture.OneTimeSetUp();

        _busDatabaseTestFixture = new DatabaseTestFixture("Read_Database_Test");
        await _busDatabaseTestFixture.OneTimeSetUp();

        _applicationDatabaseTestFixture = new DatabaseTestFixture("Write_Database_Test");
        await _applicationDatabaseTestFixture.OneTimeSetUp();
    }

    [SetUp]
    protected virtual void SetUp()
    {
        _busTestFixture!.SetUp();
        _busDatabaseTestFixture!.SetUp();
        _applicationDatabaseTestFixture!.SetUp();
    }

    [TearDown]
    protected virtual void TearDown()
    {
        _busTestFixture?.TearDown();
        _busDatabaseTestFixture?.TearDown();
        _applicationDatabaseTestFixture?.TearDown();
    }

    [OneTimeTearDown]
    protected async virtual Task OneTimeTearDown()
    {
        await _busTestFixture!.OneTimeTearDown();
        await _busDatabaseTestFixture!.OneTimeTearDown();
        await _applicationDatabaseTestFixture!.OneTimeTearDown();
    }

    protected override Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();

        foreach (var additionalBusConfiguration in _busTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalBusConfiguration.Key, additionalBusConfiguration.Value);
        }

        foreach (var additionalDatabaseConfiguration in _applicationDatabaseTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalDatabaseConfiguration.Key, additionalDatabaseConfiguration.Value);
        }

        return additional;
    }
}
