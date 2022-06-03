namespace Morsley.UK.People.Test.Fixture;

public abstract class PeopleTestFixture
{
    protected DockerTestFixture? DockerTestFixture;
    protected global::AutoFixture.Fixture? AutoFixture;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var configuration = GetConfiguration();
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();

        DockerTestFixture = new DockerTestFixture(settings.Username, settings.Password);
        AutoFixture = new global::AutoFixture.Fixture();
        AutoFixture.Customizations.Add(new DateOfBirthSpecimenBuilder());
        AutoFixture.Customizations.Add(new AddPersonRequestSpecimenBuilder());
        AutoFixture.Customizations.Add(new PersonSpecimenBuilder());

        try
        {
            await DockerTestFixture.RunBeforeTests();
        }
        catch (Exception e)
        {
            throw new Exception("This may be Docker related. Check Docker is running.", e);
        }
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

    protected void AddPersonToDatabase(Person person)
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = DockerTestFixture!.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var peopleTable = database.GetCollection<Person>(settings.TableName);
        peopleTable.InsertOne(person);
    }

    protected IList<Person> AddPeopleToDatabase(int numberOfPeopleToAdd)
    {
        if (numberOfPeopleToAdd <= 0) return new List<Person>();

        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = DockerTestFixture!.MongoDBConnectionString;
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

    protected IList<Person> AddTestPeopleToDatabase(string personData)
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

    private void DeleteAllPeopleFromDatabase()
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = DockerTestFixture!.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var peopleTable = database.GetCollection<Person>(settings.TableName);
        peopleTable.DeleteMany("{}");
    }

    protected Person GenerateTestPerson()
    {
        var person = AutoFixture.Create<Person>();
        person.Created = new DateTime(2001, 01, 01);
        person.Updated = null;
        return person;
    }

    protected Person GenerateTestPerson(Guid personId)
    {
        var person = GenerateTestPerson();
        // person.Id = personId;
        return person;
    }

    protected Person GenerateTestPerson(uint dateOfBirthYear, uint dateOfBirthMonth, uint dateOfBirthDay)
    {
        var person = GenerateTestPerson();
        person.DateOfBirth = new DateTime((int)dateOfBirthYear, (int)dateOfBirthMonth, (int)dateOfBirthDay, 0, 0, 0, 0, DateTimeKind.Utc);
        return person;
    }

    internal int ContainerPort
    {
        get { return DockerTestFixture!.GetContainerPort(); }
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

    protected virtual Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();
        additional.Add("MongoSettings:Port", ContainerPort.ToString());
        return additional;
    }

    protected Person GetPersonFromDatabase(Guid PersonId)
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = DockerTestFixture!.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var mongoCollectionSettings = new MongoCollectionSettings();
        var peopleTable = database.GetCollection<Person>(settings.TableName, mongoCollectionSettings);
        var options = new FindOptions();
        var person = peopleTable.Find<Person>(Person => Person.Id == PersonId, options).SingleOrDefault();
        return person;
    }

    protected long NumberOfPeopleInDatabase()
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = DockerTestFixture!.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var peopleTable = database.GetCollection<Person>(settings.TableName);
        var numberOfPeople = peopleTable.Find(_ => true).CountDocuments();
        return numberOfPeople;
    }
}