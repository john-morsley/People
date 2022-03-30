namespace Morsley.UK.People.Persistence.Integration.Tests.Auxiliaries;

internal class PersonRepositoryTests : DatabaseTestFixture
{
    protected MongoContext? MongoContext;

    public PersonRepositoryTests(string name) : base(name) { }

    [SetUp]
    public void PersonRepositoryTestsSetUp()
    {
        //var configuration = GetCurrentConfiguration();

        MongoContext = new MongoContext(Configuration);
        MongoContext.IsHealthy().Should().BeTrue();

        NumberOfPeopleInDatabase().Should().Be(0);
    }
}
