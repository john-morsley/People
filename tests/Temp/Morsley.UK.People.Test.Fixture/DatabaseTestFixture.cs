namespace Morsley.UK.People.Test.Fixture;

[TestFixture]
public class DatabaseTestFixture
{
    protected DockerTestFixture<MongoDBInDocker>? DockerTestFixture;
    protected global::AutoFixture.Fixture? AutoFixture;

    protected IConfiguration? _configuration;
    protected readonly string _persistenceKey;
    private string _name;

    public IConfiguration? Configuration => _configuration;

    public DatabaseTestFixture(string name , string persistenceKey)
    {
        if (name == null) throw new ArgumentNullException("name");
        if (name.Length == 0) throw new ArgumentOutOfRangeException("name");

        AutoFixture = new global::AutoFixture.Fixture();
        AutoFixture.Customizations.Add(new DateOfBirthSpecimenBuilder());
        //AutoFixture.Customizations.Add(new AddPersonRequestSpecimenBuilder());
        AutoFixture.Customizations.Add(new PersonSpecimenBuilder());

        _name = name;
        _persistenceKey = persistenceKey;
    }

    public DatabaseTestFixture(string name, IConfiguration configuration, string persistenceKey)
    {
        if (name == null) throw new ArgumentNullException("name");
        if (name.Length == 0) throw new ArgumentOutOfRangeException("name");
        if (configuration == null) throw new ArgumentNullException("configuration");

        AutoFixture = new global::AutoFixture.Fixture();
        AutoFixture.Customizations.Add(new DateOfBirthSpecimenBuilder());
        //AutoFixture.Customizations.Add(new AddPersonRequestSpecimenBuilder());
        AutoFixture.Customizations.Add(new PersonSpecimenBuilder());

        _name = name;
        _configuration = configuration;
        _persistenceKey = persistenceKey;
    }

    [OneTimeSetUp]
    public async Task CreateDatabase()
    {
        var potentialPort = _configuration[$"{_persistenceKey}:Port"];
        var username = _configuration[$"{_persistenceKey}:Username"];
        var password = _configuration[$"{_persistenceKey}:Password"];

        if (!int.TryParse(potentialPort, out var port))
        {
            throw new NotImplementedException("Port was not a number!");
        }

        DockerTestFixture = new DockerTestFixture<MongoDBInDocker>(_name, username, password, port);

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

    private void UpdateConfiguration()
    {
        var builder = new ConfigurationBuilder().AddConfiguration(_configuration);
        builder.AddInMemoryCollection(GetInMemoryConfiguration());
        var configuration = builder.Build();
        var potentialPort = configuration[$"{_persistenceKey}:Port"];
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

    [SetUp]
    public virtual void SetUp()
    {
        DeleteAllPeopleFromDatabase();
    }

    [TearDown]
    public virtual void TearDown()
    {
        DeleteAllPeopleFromDatabase();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await DockerTestFixture!.RunAfterTests();
    }

    protected int ContainerPort
    {
        get
        {
            if (DockerTestFixture == null) throw new InvalidOperationException("Docker has not yet been initialised!");
            return DockerTestFixture!.GetContainerPort();
        }
    }

    private string GetDatabaseName()
    {
        var databaseName = _configuration[$"{_persistenceKey}:DatabaseName"];
        return databaseName;
    }

    private string GetTableName()
    {
        var tableName = _configuration[$"{_persistenceKey}:TableName"];
        return tableName;
    }

    private void DeleteAllPeopleFromDatabase()
    {
        var connectionString = GetConnectionString();
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(GetDatabaseName());
        var peopleTable = database.GetCollection<Person>(GetTableName());
        peopleTable.DeleteMany("{}");
    }

    private string GetConnectionString()
    {
        var mongoDbInDocker = DockerTestFixture!.InDocker as MongoDBInDocker;
        return mongoDbInDocker!.ConnectionString();
    }

    public Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();
        if (DockerTestFixture != null) additional.Add($"{_persistenceKey}:Port", ContainerPort.ToString());
        return additional;
    }

    //protected IConfiguration GetConfiguration(Dictionary<string, string>? additional = null)
    //{
    //    var builder = new ConfigurationBuilder();

    //    builder.AddJsonFile("appsettings.json");

    //    if (additional != null && additional.Count > 0) builder.AddInMemoryCollection(additional);

    //    IConfiguration configuration = builder.Build();

    //    return configuration;
    //}

    //protected IConfiguration GetCurrentConfiguration()
    //{
    //var additional = GetInMemoryConfiguration();
    //var configuration = GetConfiguration(additional);
    //return configuration;
    //}

    public IList<Person> AddTestPeopleToDatabase(string personData)
    {
        var people = new List<Person>();

        foreach (var personDatam in personData.Split('|'))
        {
            var builder = new PersonBuilder();
            var fields = personDatam.Split(',');
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
            var person = builder.Build();
            AddPersonToDatabase(person);
            people.Add(person);
        }

        return people;
    }

    public void AddPersonToDatabase(Person person)
    {
        var connectionString = GetConnectionString();
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(GetDatabaseName());
        var peopleTable = database.GetCollection<Person>(GetTableName());
        peopleTable.InsertOne(person);
    }

    public IList<Person> AddPeopleToDatabase(int numberOfPeopleToAdd)
    {
        if (numberOfPeopleToAdd <= 0) return new List<Person>();

        var connectionString = GetConnectionString();
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(GetDatabaseName());
        var peopleTable = database.GetCollection<Person>(GetTableName());
        var people = new List<Person>();
        for (var i = 0; i < numberOfPeopleToAdd; i++)
        {
            var person = GenerateTestPerson();
            people.Add(person);
        }
        peopleTable.InsertMany(people);
        return people;
    }

    //protected string GetTestString(string prepend = "")
    //{
    //    return prepend + AutoFixture.Create<string>();
    //}

    public Person GenerateTestPerson()
    {
        var person = AutoFixture.Create<Person>();
        person.Created = DateTime.UtcNow;
        person.Updated = null;
        return person;
    }

    //    //protected Person GenerateTestPerson(Guid PersonId)
    //    //{
    //    //    var Person = GenerateTestPerson();
    //    //    //Person.Id = PersonId;
    //    //    return Person;
    //    //}

    //    protected Person GenerateTestPerson(uint dateOfBirthYear, uint dateOfBirthMonth, uint dateOfBirthDay)
    //    {
    //        var Person = GenerateTestPerson();
    //        Person.DateOfBirth = new DateTime((int)dateOfBirthYear, (int)dateOfBirthMonth, (int)dateOfBirthDay, 0, 0, 0, 0, DateTimeKind.Utc);
    //        return Person;
    //    }

    public Person GetPersonFromDatabase(Guid personId)
    {
        var connectionString = GetConnectionString();
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(GetDatabaseName());
        var mongoCollectionSettings = new MongoCollectionSettings();
        var peopleTable = database.GetCollection<Person>(GetTableName(), mongoCollectionSettings);
        var options = new FindOptions();
        var person = peopleTable.Find<Person>(Person => Person.Id == personId, options).SingleOrDefault();
        return person;
    }

    // Delay & retries are a crude mechanism to resolve eventual consistency
    public long NumberOfPeople(int delayInMilliSeconds = 0, int maximumNumberOfRetries = 1, long? expectedResult = null)
    {
        var numberOfPeople = 0L;
        for (var i = 0; i < maximumNumberOfRetries; i++)
        {
            if (delayInMilliSeconds > 0) Task.Delay(delayInMilliSeconds);
            var connectionString = GetConnectionString();
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase(GetDatabaseName());
            var peopleTable = database.GetCollection<Person>(GetTableName());
            numberOfPeople = peopleTable.Find(_ => true).CountDocuments();
            if (expectedResult is not null)
            {
                if (numberOfPeople == expectedResult) return numberOfPeople;
            }
        }
        return numberOfPeople;
    }
}