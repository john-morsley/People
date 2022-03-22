using Morsley.UK.People.Test.Fixture;
using System.Net.NetworkInformation;
using xMorsley.UK.People.Test.Fixturex;

namespace xMorsley.UK.People.API.Test.Fixturex;

public class xAPITestFixturex<TStartUp> : xPeopleTestFixturex where TStartUp : class
{
    protected const string API_MEDIA_TYPE = "application/json";

    protected MongoContext? _mongoContext;

    private TestServer? _server;
    protected HttpClient? _client;

    [SetUp]
    public override void SetUp()
    {
        var configuration = GetCurrentConfiguration();
        _mongoContext = new MongoContext(configuration);
        _mongoContext.IsHealthy().Should().BeTrue();

        NumberOfPeopleInDatabase().Should().Be(0);

        var webHostBuilder = new WebHostBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json");
                builder.AddInMemoryCollection(GetInMemoryConfiguration());
            })
            .UseEnvironment("Development")
            .UseStartup<TStartUp>();

        _server = new TestServer(webHostBuilder);
        _client = _server.CreateClient();
        if (_client == null) throw new InvalidOperationException("Cannot create HttpClient.");

        base.SetUp();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();

        _client?.Dispose();
        _server?.Dispose();
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

    private static IList<string> AllUserFields<T>() where T : class
    {
        var userFields = new List<string>();

        var propertyInfos = typeof(T).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        foreach (var propertyInfo in propertyInfos)
        {
            userFields.Add(propertyInfo.Name);
        }

        return userFields;
    }

    protected static string BuildExpectedUrl(
        int pageNumber,
        int pageSize, 
        string? fields = null, 
        string? filter = null, 
        string? search = null, 
        string? sort = null)
    {
        const string baseUrl = "http://localhost/api/v1/users";

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

        var expectedUrl = $"{baseUrl}" +
                          $"?pageNumber={pageNumber}&pageSize={pageSize}" +
                          $"{fieldsParameter}" +
                          $"{filterParameter}" +
                          $"{searchParameter}" +
                          $"{sortParameter}";
        
        return expectedUrl;
    }

    protected static PersonResource? DeserializeUserResource(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        return JsonSerializer.Deserialize<PersonResource>(json, options);
    }

    protected static (IList<string> Expected, IList<string> Unexpected) DetermineExpectedAndUnexpectedFields(string validFields)
    {
        var expectedFields = new List<string>();
        var unexpectedFields = AllUserFields<PersonResponse>();

        foreach (var expectedField in validFields.Split(','))
        {
            expectedFields.Add(expectedField);
            unexpectedFields.Remove(expectedField);
        }

        unexpectedFields.Remove("Links");

        return (expectedFields, unexpectedFields);
    }

    protected AddPersonRequest GenerateTestAddPersonRequest()
    {
        //return _autoFixture.Create<AddPersonRequest>();
        throw new NotImplementedException();
    }

    protected UpdatePersonRequest GenerateTestUpdatePersonRequest(Sex? sex = null, Gender? gender = null, string? dateOfBirth = null)
    {
        //var testUpdatePerson = _autoFixture.Create<UpdatePersonRequest>();
        //if (testUpdatePerson.Sex == sex) testUpdatePerson.Sex = GenerateDifferentSex(sex);
        //if (testUpdatePerson.Gender == gender) testUpdatePerson.Gender = GenerateDifferentGender(gender);
        //testUpdatePerson.DateOfBirth = GenerateDifferentDateOfBirth(dateOfBirth);
        //return testUpdatePerson;
        throw new NotImplementedException();
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
                case "System.DateTime": return ((DateTime)value).ToString("yyyy-MM-dd");
            }
        }

        return value;
    }

    protected void LinksForPageOfUsersShouldBeCorrect(
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
            var previousPageOfUsersLink = links.Single(_ => _.Relationship == "previous" && _.Method == "GET");
            previousPageOfUsersLink.Should().NotBeNull();
            expectedUrl = BuildExpectedUrl(pageNumber - 1, pageSize, fields, filter, search, sort);

            var previousPageOfUsersUrl = HttpUtility.UrlDecode(previousPageOfUsersLink.HypertextReference);
            previousPageOfUsersUrl.Should().Be(expectedUrl);
        }

        // Current page...
        var currentPageOfUsersLink = links.Single(_ => _.Relationship == "self" && _.Method == "GET");
        currentPageOfUsersLink.Should().NotBeNull();
        expectedUrl = BuildExpectedUrl(pageNumber, pageSize, fields, filter, search, sort);

        var currentPageOfUsersUrl = HttpUtility.UrlDecode(currentPageOfUsersLink.HypertextReference);
        currentPageOfUsersUrl.Should().Be(expectedUrl);

        // Next page...
        if (totalNumber > pageSize * pageNumber)
        {
            var nextPageOfUsersLink = links.Single(_ => _.Relationship == "next" && _.Method == "GET");
            nextPageOfUsersLink.Should().NotBeNull();
            expectedUrl = BuildExpectedUrl(pageNumber + 1, pageSize, fields, filter, search, sort);

            var nextPageOfUsersUrl = HttpUtility.UrlDecode(nextPageOfUsersLink.HypertextReference);
            nextPageOfUsersUrl.Should().Be(expectedUrl);
        }
    }

    protected static void LinksForUserShouldBeCorrect(IList<Link> links, Guid userId)
    {
        links.Should().NotBeNull();
        links.Count.Should().Be(2);

        var getUserLink = links.Single(_ => _.Method == "GET" && _.Relationship == "self");
        getUserLink.Should().NotBeNull();
        getUserLink.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");

        var deleteUserLink = links.Single(_ => _.Method == "DELETE" && _.Relationship == "self");
        deleteUserLink.Should().NotBeNull();
        deleteUserLink.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");
    }

    protected static void LinksForUsersShouldBeCorrect(IList<PersonResource>? embedded)
    {
        foreach (var userData in embedded)
        {
            userData.Should().NotBeNull();
            userData.Data.Should().NotBeNull();
            userData.Links.Should().NotBeNull();
            userData.Embedded.Should().BeNull();
            var userId = userData.Data.Id;
            LinksForUserShouldBeCorrect(userData.Links, userId);
        }
    }

    protected void ShouldBeEquivalentTo(PersonResource resource, Person user)
    {
        resource.Should().NotBeNull();
        user.Should().NotBeNull();
        resource.Data.Should().NotBeNull();
        resource.Data.Id.Should().Be(user.Id);
        resource.Data.FirstName.Should().Be(user.FirstName);
        resource.Data.LastName.Should().Be(user.LastName);
        if (user.DateOfBirth.HasValue)
        {
            resource.Data.DateOfBirth.Should().Be(user.DateOfBirth?.ToString("yyyy-MM-dd"));
        }
        resource.Data.Sex.Should().Be(user.Sex);
        resource.Data.Gender.Should().Be(user.Gender);
    }

    protected void ShouldBeEquivalentTo(IList<PersonResource> embedded, Person user)
    {
        var users = new List<Person> { user };
        ShouldBeEquivalentTo(embedded, users);
    }

    protected void ShouldBeEquivalentTo(IList<PersonResource> embedded, IList<Person> users)
    {
        foreach (var resource in embedded)
        {
            resource.Should().NotBeNull();
            var embeddedUser = resource.Data;
            var userId = embeddedUser.Id;
            var user = users.Single(_ => _.Id == userId);
            user.Should().NotBeNull();
            ShouldBeEquivalentTo(resource, user);
            //ShoudBeEquivalentTo(resource, user);
            //ShouldBeCorrect(resource.Links, userId);
        }
    }

    //private void ShoudBeEquivalentTo(PersonResource resource, Users.Domain.Models.User user)
    //{
    //    throw new NotImplementedException();
    //}

    //protected void ShouldBeEquivalentTo(IEnumerable<PersonResource> embedded, Users.Domain.Models.User user)
    //{
    //    var users = new List<Users.Domain.Models.User> { user };
    //    ShoudBeEquivalentTox(embedded, users);
    //}

    //protected void ShouldBeEquivalentTo(IEnumerable<PersonResource> embedded, IList<Users.Domain.Models.User> users)
    //{
    //    foreach (var resource in embedded)
    //    {
    //        var embeddedUser = resource.User;
    //        var userId = embeddedUser.Id;
    //        var user = users.SingleOrDefault(_ => _.Id == userId);
    //        //ShoudBeEquivalentTo(resource, user);
    //        ShouldBeCorrect(resource.Links, userId);
    //    }
    //}

    //protected void ShoudBeEquivalentTo(PersonResource resource, Users.Domain.Models.User user)
    //{
    //userResponse.Should().NotBeNull();
    //user.Should().NotBeNull();

    //userResponse.Id.Should().Be(user.Id);
    //userResponse.FirstName.Should().Be(user.FirstName);
    //userResponse.LastName.Should().Be(user.LastName);
    //if (string.IsNullOrEmpty(userResponse.DateOfBirth))
    //{
    //    user.DateOfBirth.Should().BeNull();
    //}
    //else
    //{
    //    user.DateOfBirth.Should().NotBeNull();
    //    userResponse.DateOfBirth.Should().Be(user.DateOfBirth?.ToString("yyyy-MM-dd"));
    //}
    //userResponse.Sex.Should().Be(user.Sex);
    //userResponse.Gender.Should().Be(user.Gender);
    //}
}