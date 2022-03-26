namespace Morsley.UK.People.Test.Fixture;

[TestFixture]
public class PeopleTestFixture
{
    //private const string TestPersonData = "FirstName:Charles,LastName:Babbage,Sex:Male|" +
    //                                    "FirstName:George,LastName:Boole,Sex:Male|" +
    //                                    "FirstName:John,LastName:Cleese,Sex:Male|" +
    //                                    "FirstName:Jane,LastName:Doe,Sex:Female|" +
    //                                    "FirstName:John,LastName:Doe,Sex:Male|" +
    //                                    "FirstName:Tommy,LastName:Flowers,Sex:Male|" +
    //                                    "FirstName:Jane,LastName:Goodall,Sex:Female|" +
    //                                    "FirstName:Ada,LastName:Lovelace,Sex:Female|" +
    //                                    "FirstName:Linus,LastName:Torvalds,Sex:Male|" +
    //                                    "FirstName:Alan,LastName:Turing,Sex:Male";

    protected DockerTestFixture? DockerTestFixture;
    protected global::AutoFixture.Fixture? AutoFixture;

    protected IConfiguration? Configuration;

    protected void LoadInitialConfiguration()
    {
        Configuration = GetConfiguration();
    }

    [OneTimeSetUp]
    protected async Task OneTimeSetUp()
    {
        LoadInitialConfiguration();

        var section = Configuration!.GetSection(nameof(MongoSettings));
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

        LoadAdditionalConfiguration();
    }

    private void LoadAdditionalConfiguration()
    {
        var additionalConfiguration = GetInMemoryConfiguration();
        Configuration = GetConfiguration(additionalConfiguration);
    }

    [SetUp]
    protected virtual void SetUp()
    {
        DeleteAllPeopleFromDatabase();
    }

    [TearDown]
    protected virtual void TearDown()
    {
        DeleteAllPeopleFromDatabase();
    }

    [OneTimeTearDown]
    protected async Task OneTimeTearDown()
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
        //var additional = GetInMemoryConfiguration();
        //var configuration = GetConfiguration(additional);
        var section = Configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = DockerTestFixture!.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var peopleTable = database.GetCollection<Person>(settings.TableName);
        peopleTable.DeleteMany("{}");
    }

    protected virtual Dictionary<string, string> GetInMemoryConfiguration()
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

    protected void AddPersonToDatabase(Person person)
    {
        //var additional = GetInMemoryConfiguration();
        //var configuration = GetConfiguration(additional);
        var section = Configuration.GetSection(nameof(MongoSettings));
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

        //var additional = GetInMemoryConfiguration();
        //var configuration = GetConfiguration(additional);
        var section = Configuration.GetSection(nameof(MongoSettings));
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

    protected string GetTestString(string prepend = "")
    {
        return prepend + AutoFixture.Create<string>();
    }

    protected string? GenerateDifferentDateOfBirth(string? dateOfBirth)
    {
        if (dateOfBirth == null ||
            !DateTime.TryParseExact(dateOfBirth, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt))
        {
            return "2001-02-03";
        }
        return dt.ToString("yyyy-MM-dd");
    }

    protected Gender? GenerateDifferentGender(Gender? gender)
    {
        Gender? differentGender;
        do
        {
            differentGender = AutoFixture.Create<Gender?>();
        } while (differentGender == gender);

        return differentGender;
    }

    protected Sex? GenerateDifferentSex(Sex? sex)
    {
        Sex? differentSex;
        do
        {
            differentSex = AutoFixture.Create<Sex?>();
        } while (differentSex == sex);

        return differentSex;
    }

    protected Person GenerateTestPerson()
    {
        var person = AutoFixture.Create<Person>();
        person.Created = new DateTime(2001, 01, 01);
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

    protected Person GetPersonFromDatabase(Guid PersonId)
    {
        //var additional = GetInMemoryConfiguration();
        //var configuration = GetConfiguration(additional);
        var section = Configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = DockerTestFixture!.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var mongoCollectionSettings = new MongoCollectionSettings();
        var peopleTable = database.GetCollection<Person>(settings.TableName, mongoCollectionSettings);
        var options = new FindOptions();
        var Person = peopleTable.Find<Person>(Person => Person.Id == PersonId, options).SingleOrDefault();
        return Person;
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

    protected long NumberOfPeopleInDatabase()
    {
        //var additional = GetInMemoryConfiguration();
        //var configuration = GetConfiguration(additional);
        var section = Configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = DockerTestFixture!.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var peopleTable = database.GetCollection<Person>(settings.TableName);
        var numberOfPeople = peopleTable.Find(_ => true).CountDocuments();
        return numberOfPeople;
    }
}