namespace Morsley.UK.People.API.Test.Fixture;

// Responsible for:
// 1. The SUT --> The Read application
// 2. Creating a Docker MongoDB instance for the SUT
public class ReadApplicationTestFixture<TProgram> : SecuredApplicationTestFixture<TProgram> where TProgram : class
{
    public DatabaseTestFixture ApplicationDatabase => _databaseTestFixture!;

    protected DatabaseTestFixture? _databaseTestFixture;

    public ReadApplicationTestFixture()
    {
        
    }

    [OneTimeSetUp]
    protected async override Task OneTimeSetUp()
    {
        _databaseTestFixture = new DatabaseTestFixture("Read_Database_Test");
        await _databaseTestFixture.OneTimeSetUp();
    }

    [SetUp]
    protected virtual void SetUp()
    {
        _databaseTestFixture!.SetUp();
    }

    [TearDown]
    protected virtual void TearDown()
    {
        _databaseTestFixture?.TearDown();
    }

    [OneTimeTearDown]
    protected async virtual Task OneTimeTearDown()
    {
        await _databaseTestFixture!.OneTimeTearDown();
    }

    protected override Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();

        foreach (var additionalDatabaseConfiguration in _databaseTestFixture!.GetInMemoryConfiguration())
        {
            additional.Add(additionalDatabaseConfiguration.Key, additionalDatabaseConfiguration.Value);
        }

        return additional;
    }
}
