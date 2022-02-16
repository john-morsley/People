namespace Shared.TestInfrastructure;

[TestFixture]
public class TestBase
{
    //private const string TestUserData = "FirstName:Charles,LastName:Babbage,Sex:Male|" +
    //                                    "FirstName:George,LastName:Boole,Sex:Male|" +
    //                                    "FirstName:John,LastName:Cleese,Sex:Male|" +
    //                                    "FirstName:Jane,LastName:Doe,Sex:Female|" +
    //                                    "FirstName:John,LastName:Doe,Sex:Male|" +
    //                                    "FirstName:Tommy,LastName:Flowers,Sex:Male|" +
    //                                    "FirstName:Jane,LastName:Goodall,Sex:Female|" +
    //                                    "FirstName:Ada,LastName:Lovelace,Sex:Female|" +
    //                                    "FirstName:Linus,LastName:Torvalds,Sex:Male|" +
    //                                    "FirstName:Alan,LastName:Turing,Sex:Male";
                                    
    protected TestFixture _testFixture;
    protected Fixture _autoFixture;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var configuration = GetConfiguration();
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();

        _testFixture = new TestFixture(settings.Username, settings.Password);
        _autoFixture = new Fixture();
        _autoFixture.Customizations.Add(new DateOfBirthSpecimenBuilder());
        _autoFixture.Customizations.Add(new AddUserRequestSpecimenBuilder());
        _autoFixture.Customizations.Add(new UserSpecimenBuilder());

        await _testFixture.RunBeforeTests();
    }

    [SetUp]
    public virtual void SetUp()
    {
        DeleteAllUsersFromDatabase();
    }

    private void DeleteAllUsersFromDatabase()
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var usersTable = database.GetCollection<Users.Domain.Models.User>(settings.TableName);
        usersTable.DeleteMany("{}");
    }

    [TearDown]
    public virtual void TearDown()
    {
        DeleteAllUsersFromDatabase();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
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

    protected IList<Users.Domain.Models.User> AddTestUsersToDatabase(string userData)
    {
        var users = new List<Users.Domain.Models.User>();

        foreach(var userDatam in userData.Split('|')) 
        {
            var builder = new Users.Domain.Builders.UserBuilder();
            var fields = userDatam.Split(',');
            var name = fields[0].Split(' ');
            builder.SetFirstName(name[0]);
            builder.SetLastName(name[1]);
            foreach (var field in fields.Skip(1))
            {
                var kvp = field.Split(':');
                var key = kvp[0];
                var value = kvp[1];
                switch (key)
                {
                    case "Sex": builder.SetSex(value); break;
                    case "Gender": builder.SetGender(value); break;
                    case "DateOfBirth": builder.SetDateOfBirth(value); break;
                }
            }
            var user = builder.Build();
            AddUserToDatabase(user);
            users.Add(user);
        }

        return users;
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
        var usersTable = database.GetCollection<Users.Domain.Models.User>(settings.TableName);
        usersTable.InsertOne(user);
    }

    protected IList<Users.Domain.Models.User> AddUsersToDatabase(int numberOfUsersToAdd)
    {
        if (numberOfUsersToAdd <= 0) return new List<Users.Domain.Models.User>();

        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var usersTable = database.GetCollection<Users.Domain.Models.User>(settings.TableName);
        var users = new List<Users.Domain.Models.User>();
        for (int i = 0; i < numberOfUsersToAdd; i++)
        {
            var user = GenerateTestUser();
            users.Add(user);
        }
        usersTable.InsertMany(users);
        return users;
    }

    protected string GetTestString(string prepend = "")
    {
        return prepend + _autoFixture.Create<string>();
    }

    protected Gender? GenerateDifferentGender(Gender? gender)
    {
        Gender? differentGender;
        do
        {
            differentGender = _autoFixture.Create<Gender?>();
        } while (differentGender == gender);

        return differentGender;
    }

    protected Sex? GenerateDifferentSex(Sex? sex)
    {
        Sex? differentSex;
        do
        {
            differentSex = _autoFixture.Create<Sex?>();
        } while (differentSex == sex);
        
        return differentSex;
    }

    protected Users.Domain.Models.User GenerateTestUser()
    {
        var user = _autoFixture.Create<Users.Domain.Models.User>();
        user.Created = DateTime.UtcNow;
        user.Updated = null;
        return user;
    }

    protected Users.Domain.Models.User GenerateTestUser(Guid userId)
    {
        var user = GenerateTestUser();
        user.Id = userId;
        return user;
    }

    protected Users.Domain.Models.User GenerateTestUser(uint dateOfBirthYear, uint dateOfBirthMonth, uint dateOfBirthDay)
    {
        var user = GenerateTestUser();
        user.DateOfBirth = new DateTime((int)dateOfBirthYear, (int)dateOfBirthMonth, (int)dateOfBirthDay, 0, 0, 0, 0, DateTimeKind.Utc);
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
        var usersTable = database.GetCollection<Users.Domain.Models.User>(settings.TableName);
        var user = usersTable.Find<Users.Domain.Models.User>(user => user.Id == userId).SingleOrDefault();
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
        var usersTable = database.GetCollection<Users.Domain.Models.User>(settings.TableName);
        return usersTable.AsQueryable();
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
        var usersTable = database.GetCollection<Users.Domain.Models.User>(settings.TableName);
        var numberOfUsers = usersTable.Find(_ => true).CountDocuments();
        return numberOfUsers;
    }
}
