namespace Morsley.UK.People.Test.Fixture;

[TestFixture]
public class DatabaseTestFixture
{
    protected DockerTestFixture<MongoDBInDocker>? DockerTestFixture;
    protected global::AutoFixture.Fixture? AutoFixture;

    public IConfiguration? Configuration;

    private string _name;

    public DatabaseTestFixture(string name)
    {
        AutoFixture = new global::AutoFixture.Fixture();
        AutoFixture.Customizations.Add(new DateOfBirthSpecimenBuilder());
        //AutoFixture.Customizations.Add(new AddPersonRequestSpecimenBuilder());
        AutoFixture.Customizations.Add(new PersonSpecimenBuilder());

        _name = name;
    }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        LoadInitialConfiguration();

        var section = Configuration!.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();

        if (!int.TryParse(settings.Port, out var port))
        {
            throw new NotImplementedException("Port was not a number!");
        }

        DockerTestFixture = new DockerTestFixture<MongoDBInDocker>(_name, settings.Username!, settings.Password!, port);

        try
        {
            await DockerTestFixture.RunBeforeTests();
        }
        catch (Exception e)
        {
            throw new Exception("This may be Docker related. Check Docker is running.", e);
        }

        LoadAdditionalConfiguration();

        if (Configuration == null) Assert.Fail("Configuration should not be null!");
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

    private void DeleteAllPeopleFromDatabase()
    {
        var section = Configuration!.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = GetConnectionString();
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var peopleTable = database.GetCollection<Person>(settings.TableName);
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
        if (DockerTestFixture != null) additional.Add("MongoSettings:Port", ContainerPort.ToString());
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
        var section = Configuration!.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = GetConnectionString();
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var peopleTable = database.GetCollection<Person>(settings.TableName);
        peopleTable.InsertOne(person);
    }

    public IList<Person> AddPeopleToDatabase(int numberOfPeopleToAdd)
    {
        if (numberOfPeopleToAdd <= 0) return new List<Person>();

        var section = Configuration!.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = GetConnectionString();
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var peopleTable = database.GetCollection<Person>(settings.TableName);
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
        var section = Configuration!.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = GetConnectionString();
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var mongoCollectionSettings = new MongoCollectionSettings();
        var peopleTable = database.GetCollection<Person>(settings.TableName, mongoCollectionSettings);
        var options = new FindOptions();
        var person = peopleTable.Find<Person>(Person => Person.Id == personId, options).SingleOrDefault();
        return person;
    }

    //    protected IQueryable<Person> GetPeopleFromDatabase()
    //    {
    //        var additional = GetInMemoryConfiguration();
    //        var configuration = GetConfiguration(additional);
    //        var section = configuration.GetSection(nameof(MongoSettings));
    //        var settings = section.Get<MongoSettings>();
    //        var connectionString = _testFixture!.MongoDBConnectionString;
    //        var mongoClient = new MongoClient(connectionString);
    //        var database = mongoClient.GetDatabase(settings.DatabaseName);
    //        var peopleTable = database.GetCollection<Person>(settings.TableName);
    //        return peopleTable.AsQueryable();
    //    }

    private void LoadAdditionalConfiguration()
    {
        var additionalConfiguration = GetInMemoryConfiguration();
        Configuration = GetConfiguration(additionalConfiguration);
    }

    protected void LoadInitialConfiguration()
    {
        Configuration = GetConfiguration();
    }

    public long NumberOfPeopleInDatabase()
    {
        var section = Configuration!.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = GetConnectionString();
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var peopleTable = database.GetCollection<Person>(settings.TableName);
        var numberOfPeople = peopleTable.Find(_ => true).CountDocuments();
        return numberOfPeople;
    }
}