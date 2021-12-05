namespace Shared.TestInfrastructure;

public class TestBase
{
    protected TestFixture _testFixture;
    protected Fixture _autoFixture;

    [SetUp]
    public async Task TestSetUp()
    {
        var configuration = GetConfiguration();
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();

        _testFixture = new TestFixture(settings.Username, settings.Password);
        _autoFixture = new Fixture();

        await _testFixture.RunBeforeTests();
    }

    [TearDown]
    public async Task TestTearDown()
    {
        await _testFixture.RunAfterTests();
    }

    internal int ContainerPort
    {
        get { return _testFixture.GetContainerPort(); }
    }

    protected virtual Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();
        additional.Add("MongoSettings:Port", ContainerPort.ToString());
        return additional;
    }

    protected IConfiguration GetConfiguration(Dictionary<string, string>? additional = null)
    {
        var builder = new ConfigurationBuilder();

        builder.AddJsonFile("appsettings.json");

        if (additional != null && additional.Count > 0) builder.AddInMemoryCollection(additional);

        IConfiguration configuration = builder.Build();

        return configuration;
    }

    protected IConfiguration GetCurrentConfiguration()
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        return configuration;
    }

    protected void AddUserToDatabase(Users.Domain.Models.User user)
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var users = database.GetCollection<Users.Domain.Models.User>(settings.TableName);
        users.InsertOne(user);
    }

    protected void AddUsersToDatabase(int numberOfUsersToAdd)
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var collection = database.GetCollection<Users.Domain.Models.User>(settings.TableName);
        var users = new List<Users.Domain.Models.User>();
        for (int i = 0; i < numberOfUsersToAdd; i++)
        {
            var user = GenerateTestUser();
            users.Add(user);
        }
        collection.InsertMany(users);
    }

    protected Users.Domain.Models.User GenerateTestUser()
    {
        return _autoFixture.Create<Users.Domain.Models.User>();
    }

    protected Users.Domain.Models.User GenerateTestUser(Guid userId)
    {
        var user = GenerateTestUser();
        user.Id = userId;
        return user;
    }

    protected Users.Domain.Models.User GetUserFromDatabase(Guid userId)
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var users = database.GetCollection<Users.Domain.Models.User>(settings.TableName);
        var user = users.Find<Users.Domain.Models.User>(user => user.Id == userId).SingleOrDefault();
        return user;
    }

    protected IQueryable<Users.Domain.Models.User> GetUsersFromDatabase()
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var users = database.GetCollection<Users.Domain.Models.User>(settings.TableName);
        return users.AsQueryable();
    }

    protected long NumberOfUsersInDatabase()
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var users = database.GetCollection<Users.Domain.Models.User>(settings.TableName);
        var numberOfUsers = users.Find(_ => true).CountDocuments();
        return numberOfUsers;
    }
}
