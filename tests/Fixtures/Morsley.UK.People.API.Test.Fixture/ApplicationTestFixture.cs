namespace Morsley.UK.People.API.Test.Fixture;

public abstract class ApplicationTestFixture<TProgram>  where TProgram : class
{
    //protected ILogger Logger;

    //protected bool HasBus = false;
    //protected bool HasDatabase = false;

    // Test Fixture for the People Bus...
    //protected BusTestFixture? BusTestFixture;

    // Test Fixture for the People Databases...
    //protected DatabaseTestFixture? DatabaseTestFixture;
    //protected DatabaseTestFixture? WriteDatabaseTestFixture;
    //protected DatabaseTestFixture? DatabaseTestFixture;

    // The HttpClient to hit the application under test...
    protected HttpClient? HttpClient;

    private int _applicationPort;

    protected IConfiguration? _configuration;

    protected global::AutoFixture.Fixture? AutoFixture;

    protected int ApplicationPort
    {
        get
        {
            //if (_applicationPort == 0) _applicationPort = GetApplicationPort(DatabaseTestFixture.Configuration);
            if (_applicationPort == 0) _applicationPort = GetApplicationPort(GetConfiguration());
            return _applicationPort;
        }
    }
    
    protected ApplicationTestFixture()
    {
        AutoFixture = new global::AutoFixture.Fixture();
        AutoFixture.Customizations.Add(new DateOfBirthSpecimenBuilder());
        AutoFixture.Customizations.Add(new AddPersonRequestSpecimenBuilder());
    }

    [OneTimeSetUp]
    protected async virtual Task OneTimeSetUp()
    {
        await Task.CompletedTask;
    }

    [SetUp]
    protected async virtual Task SetUp()
    {
        var factory = new WebApplicationFactory<TProgram>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseUrls($"https://localhost:{ApplicationPort}");
                builder.UseKestrel();
                builder.ConfigureKestrel(options =>
                    {
                        options.ListenLocalhost(ApplicationPort);
                    }
                );
                builder.ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddConfiguration(GetConfiguration());
                });
            });
        HttpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new System.Uri($"https://localhost:{ApplicationPort}")
        });

        await Task.CompletedTask;
    }

    [TearDown]
    protected async virtual Task TearDown()
    {
        _configuration = null;
        HttpClient?.Dispose();
        await Task.CompletedTask;
    }

    [OneTimeTearDown]
    protected async virtual Task OneTimeTearDown()
    {
        _configuration = null;
        await Task.CompletedTask;
    }

    protected static string AddToFieldsIfMissing(string toAdd, string fields)
    {
        var listOfFields = new List<string>();
        var hasId = false;
        foreach (var field in fields.Split(','))
        {
            if (field.ToLower() == toAdd.ToLower()) hasId = true;
            listOfFields.Add(field);
        }
        if (!hasId) listOfFields.Add(toAdd);
        return string.Join(",", listOfFields);
    }

    private static IList<string> AllPublicFields<T>() where T : class
    {
        var fields = new List<string>();

        var properties = typeof(T).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            fields.Add(property.Name);
        }

        return fields;
    }

    protected static string BuildExpectedUrl(
        int pageNumber,
        int pageSize,
        string? fields = null,
        string? filter = null,
        string? search = null,
        string? sort = null)
    {
        const string baseUrl = "/api/people";

        var fieldsParameter = "";
        if (fields != null) fieldsParameter = $"&fields={fields}";

        var filterParameter = "";
        if (filter != null) filterParameter = $"&filter={filter}";

        var searchParameter = "";
        if (search != null) searchParameter = $"&search={search}";

        var sortParameter = $"&sort={sort}";
        if (sort == null)
        {
            sortParameter = $"&sort={Defaults.DefaultPageSort}";
        }

        var expectedUrl = 
            $"{baseUrl}" +
            $"?pageNumber={pageNumber}&pageSize={pageSize}" +
            $"{fieldsParameter}" +
            $"{filterParameter}" +
            $"{searchParameter}" +
            $"{sortParameter}";

        return expectedUrl;
    }

    protected static PersonResource? DeserializePersonResource(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceJsonConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        return JsonSerializer.Deserialize<PersonResource>(json, options);
    }

    protected static (IList<string> Expected, IList<string> Unexpected) DetermineExpectedAndUnexpectedFields(string validFields)
    {
        var expectedFields = new List<string>();
        var unexpectedFields = AllPublicFields<PersonResponse>();

        foreach (var expectedField in validFields.Split(','))
        {
            expectedFields.Add(expectedField);
            unexpectedFields.Remove(expectedField);
        }

        unexpectedFields.Remove("Links");

        return (expectedFields, unexpectedFields);
    }

    protected async Task<long> DetermineExpectedContentLength(string url)
    {
        var result = await HttpClient!.GetAsync(url);
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadAsStringAsync();
        return content.Length;
    }

    protected AddPersonRequest GenerateAddPersonRequest()
    {
        var request = AutoFixture.Create<AddPersonRequest>();
        return request;
    }

    protected UpdatePersonRequest GenerateTestUpdatePersonRequest(
        Guid personId, 
        Sex? sex = null, 
        Gender? gender = null, 
        string? dateOfBirth = null)
    {
        var testUpdatePerson = AutoFixture.Create<UpdatePersonRequest>();
        testUpdatePerson.Id = personId;
        if (testUpdatePerson.Sex == sex) testUpdatePerson.Sex = sex.GenerateDifferentSex();
        if (testUpdatePerson.Gender == gender) testUpdatePerson.Gender = gender.GenerateDifferentGender();
        testUpdatePerson.DateOfBirth = dateOfBirth.GenerateDifferentDate();
        return testUpdatePerson;
    }

    protected int GetApplicationPort(IConfiguration configuration)
    {
        var potentialPort = configuration["ApplicationPort"];

        if (string.IsNullOrEmpty(potentialPort)) throw new InvalidProgramException("Invalid configuration --> Port is missing!");

        if (int.TryParse(potentialPort, out var port)) return port;

        throw new InvalidProgramException("Invalid configuration --> port is not a number!");
    }

    public IConfiguration GetConfiguration()
    {
        if (_configuration is not null) return _configuration;

        var builder = new ConfigurationBuilder();

        builder.AddJsonFile("appsettings.json");

        var additional = GetInMemoryConfiguration();

        if (additional.Count > 0) builder.AddInMemoryCollection(additional);

        _configuration = builder.Build();

        return _configuration;
    }

    protected virtual Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();

        //if (HasBus)
        //{
        //    foreach (var additionalBusConfiguration in BusTestFixture!.GetInMemoryConfiguration())
        //    {
        //        additional.Add(additionalBusConfiguration.Key, additionalBusConfiguration.Value);
        //    }
        //}

        //if (HasDatabase)
        //{
        //    foreach (var additionalDatabaseConfiguration in DatabaseTestFixture!.GetInMemoryConfiguration())
        //    {
        //        additional.Add(additionalDatabaseConfiguration.Key, additionalDatabaseConfiguration.Value);
        //    }
        //}

        return additional;
    }

    protected static object? GetValue<T>(T t, string fieldName) where T : class
    {
        var propertyInfo = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        var type = propertyInfo?.PropertyType;
        if (type == null) return null;
        var value = propertyInfo?.GetValue(t);
        if (value == null) return null;
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null)
        {
            switch (underlying.FullName)
            {
                case "System.DateTime": 
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                case "Morsley.UK.People.Domain.Enumerations.Gender":
                case "Morsley.UK.People.Domain.Enumerations.Sex":
                    return value.ToString();
            }
        }

        return value;
    }

    protected static void LinksForPeopleShouldBeCorrect(
        IList<Link>? links,
        int pageNumber,
        int pageSize,
        int totalNumber,
        string? fields = null,
        string? filter = null,
        string? search = null,
        string? sort = null)
    {
        string expectedUrl;

        // Previous page...
        if (pageNumber > 1)
        {
            var previousPageOfPeopleLink = links.Single(_ => _.Relationship == "previous" && _.Method == "GET");
            previousPageOfPeopleLink.Should().NotBeNull();
            //expectedUrl = BuildExpectedUrl(pageNumber - 1, pageSize, fields, filter, search, sort);

            //var previousPageOfPeopleUrl = HttpUtility.UrlDecode(previousPageOfPeopleLink.HypertextReference);
            //previousPageOfPeopleUrl.Should().Be(expectedUrl);
        }

        // Current page...
        var currentPageOfPeopleLink = links.Single(_ => _.Relationship == "self" && _.Method == "GET");
        currentPageOfPeopleLink.Should().NotBeNull();
        //expectedUrl = BuildExpectedUrl(pageNumber, pageSize, fields, filter, search, sort);

        //var currentPageOfPeopleUrl = HttpUtility.UrlDecode(currentPageOfPeopleLink.HypertextReference);
        //currentPageOfPeopleUrl.Should().Be(expectedUrl);

        // Next page...
        if (totalNumber > pageSize * pageNumber)
        {
            var nextPageOfPeopleLink = links.Single(_ => _.Relationship == "next" && _.Method == "GET");
            nextPageOfPeopleLink.Should().NotBeNull();
        //    expectedUrl = BuildExpectedUrl(pageNumber + 1, pageSize, fields, filter, search, sort);

        //    var nextPageOfPeopleUrl = HttpUtility.UrlDecode(nextPageOfPeopleLink.HypertextReference);
        //    nextPageOfPeopleUrl.Should().Be(expectedUrl);
        }
    }

    protected static void LinksForPersonShouldBeCorrect(IList<Link> links, Guid personId)
    {
        links.Should().NotBeNull();
        links.Count.Should().Be(3);

        var getPersonLink = links.Single(_ => _.Method == "GET" && _.Relationship == "self");
        getPersonLink.Should().NotBeNull();
        getPersonLink.HypertextReference.Should().Be($"/api/person/{personId}");

        var updatePersonLink = links.Single(_ => _.Method == "PUT" && _.Relationship == "self");
        updatePersonLink.Should().NotBeNull();
        updatePersonLink.HypertextReference.Should().Be($"/api/person/{personId}");

        var deletePersonLink = links.Single(_ => _.Method == "DELETE" && _.Relationship == "self");
        deletePersonLink.Should().NotBeNull();
        deletePersonLink.HypertextReference.Should().Be($"/api/person/{personId}");
    }

    protected static void LinksForPeopleShouldBeCorrect(IList<PersonResource>? embedded)
    {
        foreach (var person in embedded)
        {
        }
    }

    protected void ProblemDetailsShouldContainValidationIssues(string content, Dictionary<string, string> validationErrors)
    {
        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        problemDetails.Title.Should().Be("Validation error(s) occurred!");
        problemDetails.Detail.Should().Be("See the errors field for details.");
        problemDetails.Extensions.Should().NotBeNull();
        problemDetails.Extensions.Count().Should().Be(validationErrors.Count);

        foreach (var validationError in validationErrors)
        {
            var error = problemDetails.Extensions.SingleOrDefault(_ => _.Key == validationError.Key);
            error.Should().NotBeNull();
            error.Value.Should().NotBeNull();
            error.Value?.ToString().Should().Be(validationError.Value);
        }
    }

    protected void ProblemDetailsShouldContain(
        string content, 
        string title,
        string detail,
        Dictionary<string, string> issues)
    {
        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        problemDetails.Title.Should().Be(title);
        problemDetails.Detail.Should().Be(detail);
        problemDetails.Extensions.Should().NotBeNull();
        problemDetails.Extensions.Count().Should().Be(issues.Count);

        foreach (var issue in issues)
        {
            var error = problemDetails.Extensions.SingleOrDefault(_ => _.Key == issue.Key);
            error.Should().NotBeNull();
            error.Value.Should().NotBeNull();
            error.Value?.ToString().Should().Be(issue.Value);
        }
    }

    protected void ShouldBeEquivalentTo(PersonResource resource, Person person)
    {
        resource.Should().NotBeNull();
        person.Should().NotBeNull();
        resource.Data.Should().NotBeNull();
        resource.Data!.Id.Should().Be(person.Id);
        resource.Data.FirstName.Should().Be(person.FirstName);
        resource.Data.LastName.Should().Be(person.LastName);
        if (person.DateOfBirth.HasValue)
        {
            resource.Data.DateOfBirth.Should().Be(person.DateOfBirth?.ToString("yyyy-MM-dd"));
        }
        if (person.Sex.HasValue)
        {
            resource.Data.Sex.Should().Be(person.Sex.ToString());
        }
        if (person.Gender.HasValue)
        {
            resource.Data.Gender.Should().Be(person.Gender.ToString());
        }
    }

    protected void ShouldBeEquivalentTo(IList<PersonResource> embedded, IList<Person> people)
    {
        foreach (var resource in embedded)
        {
            resource.Should().NotBeNull();
            var embeddedPerson = resource.Data;
            var personId = embeddedPerson.Id;
            var person = people.Single(_ => _.Id == personId);
            person.Should().NotBeNull();
            ShouldBeEquivalentTo(resource, person);
            resource.Links.Should().NotBeNull();
            LinksForPersonShouldBeCorrect(resource.Links!, personId);
        }
    }

    protected IConfiguration UpdateConfiguration(
        IConfiguration configuration,
        IConfiguration extraConfiguration)
    {
        var builder = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddConfiguration(extraConfiguration);

        var updatedConfiguration = builder.Build();

        return updatedConfiguration;
    }
}
