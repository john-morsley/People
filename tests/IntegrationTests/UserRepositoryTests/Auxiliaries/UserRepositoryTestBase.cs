namespace UserRepositoryTests;

internal class UserRepositoryTests : TestBase
{
    protected MongoContext _mongoContext;

    [SetUp]
    public void UserRepositoryTestsSetUp()
    {
        var configuration = GetCurrentConfiguration();
        _mongoContext = new MongoContext(configuration);
        _mongoContext.IsHealthy().Should().BeTrue();

        NumberOfUsersInDatabase().Should().Be(0);
    }
}
