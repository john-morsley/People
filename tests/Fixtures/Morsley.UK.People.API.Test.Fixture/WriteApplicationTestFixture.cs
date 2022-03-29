namespace Morsley.UK.People.API.Test.Fixture;

// Responsible for:
// 1. The SUT --> The Write application
// 2. Creating a Docker MongoDB instance for the SUT
// 3. Creating a Docker RabbitMQ instance for the SUT
// 4. Creating a Docker MongoDB instance for the SUT
public class WriteApplicationTestFixture<TProgram> : SecuredApplicationTestFixture<TProgram> where TProgram : class
{
    public BusTestFixture BusTestFixture => _busTestFixture!;
    public DatabaseTestFixture DatabaseTestFixture => _databaseTestFixture!;

    protected BusTestFixture? _busTestFixture;
    protected DatabaseTestFixture? _databaseTestFixture;

    public WriteApplicationTestFixture()
    {

    }

    [OneTimeSetUp]
    protected async override Task OneTimeSetUp()
    {
        _busTestFixture = new BusTestFixture();
        await _busTestFixture.OneTimeSetUp();

        _databaseTestFixture = new DatabaseTestFixture();
        await _databaseTestFixture.OneTimeSetUp();
    }

    [SetUp]
    protected virtual void SetUp()
    {
        _busTestFixture!.SetUp();
        _databaseTestFixture!.SetUp();
    }

    [TearDown]
    protected virtual void TearDown()
    {
        _busTestFixture?.TearDown();
        _databaseTestFixture?.TearDown();
    }

    [OneTimeTearDown]
    protected async virtual Task OneTimeTearDown()
    {
        await _busTestFixture!.OneTimeTearDown();
        await _databaseTestFixture!.OneTimeTearDown();
    }

    protected override Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();

        foreach (var additionalBusConfiguration in _busTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalBusConfiguration.Key, additionalBusConfiguration.Value);
        }

        foreach (var additionalDatabaseConfiguration in _databaseTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalDatabaseConfiguration.Key, additionalDatabaseConfiguration.Value);
        }

        return additional;
    }

}
