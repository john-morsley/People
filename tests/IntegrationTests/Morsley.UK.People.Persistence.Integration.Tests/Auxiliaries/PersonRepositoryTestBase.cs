namespace Morsley.UK.People.Persistence.Integration.Tests.Auxiliaries;

internal class PersonRepositoryTests : DatabaseTestFixture
{
    protected MongoContext? MongoContext;

    public PersonRepositoryTests(string name) : base(name, "MongoDBSettings")
    {
        _configuration = GetConfiguration();
    }

    [SetUp]
    public void PersonRepositoryTestsSetUp()
    {
        //var configuration = GetConfiguration();

        MongoContext = new MongoContext(_configuration, "MongoDBSettings");
        MongoContext.IsHealthy().Should().BeTrue();

        NumberOfPeopleInDatabase().Should().Be(0);
    }

    public IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json");
        IConfiguration configuration = builder.Build();
        return configuration;
    }
}
