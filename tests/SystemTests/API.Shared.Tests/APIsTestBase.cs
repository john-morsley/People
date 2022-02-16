using System.Reflection;
using System.Web;
using Users.API.Models.Shared;

namespace API.Shared.Tests;

public class APIsTestBase<TStartUp> : TestBase where TStartUp : class
{
    protected const string API_MEDIA_TYPE = "application/json";

    protected MongoContext _mongoContext;

    private TestServer _server;
    protected HttpClient _client;

    [SetUp]
    public override void SetUp()
    {
        var configuration = GetCurrentConfiguration();
        _mongoContext = new MongoContext(configuration);
        _mongoContext.IsHealthy().Should().BeTrue();

        NumberOfUsersInDatabase().Should().Be(0);

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

        base.SetUp();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();

        _client.Dispose();
        _server.Dispose();
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
            sortParameter = $"&sort={Users.API.Models.Constants.Defaults.DefaultPageSort}";
        }

        var expectedUrl = $"{baseUrl}" +
                          $"?pageNumber={pageNumber}&pageSize={pageSize}" +
                          $"{fieldsParameter}" +
                          $"{filterParameter}" +
                          $"{searchParameter}" +
                          $"{sortParameter}";
        
        return expectedUrl;
    }

    protected static Users.API.Models.Shared.UserData DeserializeUserData(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new Users.API.Models.Shared.UserDataConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        return JsonSerializer.Deserialize<Users.API.Models.Shared.UserData>(json, options);
    }

    protected static (IList<string> Expected, IList<string> Unexpected) DetermineExpectedAndUnexpectedFields(string validFields)
    {
        var expectedFields = new List<string>();
        var unexpectedFields = AllUserFields<Users.API.Models.Response.v1.UserResponse>();

        foreach (var expectedField in validFields.Split(','))
        {
            expectedFields.Add(expectedField);
            unexpectedFields.Remove(expectedField);
        }

        unexpectedFields.Remove("Links");

        return (expectedFields, unexpectedFields);
    }

    protected Users.API.Models.Request.v1.AddUserRequest GenerateTestAddUserRequest()
    {
        return _autoFixture.Create<Users.API.Models.Request.v1.AddUserRequest>();
    }

    protected Users.API.Models.Request.v1.UpdateUserRequest GenerateTestUpdateUserRequest(Sex? sex = null, Gender? gender = null)
    {
        var testUpdateUser = _autoFixture.Create<Users.API.Models.Request.v1.UpdateUserRequest>();
        if (testUpdateUser.Sex == sex) testUpdateUser.Sex = GenerateDifferentSex(sex);
        if (testUpdateUser.Gender == gender) testUpdateUser.Gender = GenerateDifferentGender(gender);
        return testUpdateUser;
    }

    protected static object GetValue<T>(T t, string fieldName) where T : class
    {
        var propertyInfo = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        var type = propertyInfo.PropertyType;
        var value = propertyInfo.GetValue(t);
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
        IList<Users.API.Models.Shared.Link> links, 
        int pageNumber, 
        int pageSize, 
        string? fields = null,
        string? filter = null,
        string? search = null,
        string? sort = null)
    {
        // Previous page...
        if (pageNumber < 1)
        {

        }

        // Current page...
        var currentPageOfUsersLink = links.Single(_ => _.Relationship == "self" && _.Method == "GET");
        currentPageOfUsersLink.Should().NotBeNull();
        var expectedUrl = BuildExpectedUrl(pageNumber, pageSize, fields, filter, search, sort);

        var currentPageOfUsersUrl = HttpUtility.UrlDecode(currentPageOfUsersLink.HypertextReference);
        currentPageOfUsersUrl.Should().Be(expectedUrl);

        // Next page...

    }

    protected static void LinksForUserShouldBeCorrect(IList<Users.API.Models.Shared.Link> links, Guid userId)
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

    protected static void LinksForUsersShouldBeCorrect(IList<UserData> embedded)
    {
        foreach (var userData in embedded)
        {
            userData.Should().NotBeNull();
            userData.User.Should().NotBeNull();
            userData.Links.Should().NotBeNull();
            userData.Embedded.Should().BeNull();
            var userId = userData.User.Id;
            LinksForUserShouldBeCorrect(userData.Links, userId);
        }
    }

    protected void ShouldBeEquivalentTo(Users.API.Models.Shared.UserData userData, Users.Domain.Models.User user)
    {
        userData.Should().NotBeNull();
        user.Should().NotBeNull();
        userData.User.Should().NotBeNull();
        userData.User.Id.Should().Be(user.Id);
        userData.User.FirstName.Should().Be(user.FirstName);
        userData.User.LastName.Should().Be(user.LastName);
        if (user.DateOfBirth.HasValue)
        {
            userData.User.DateOfBirth.Should().Be(user.DateOfBirth?.ToString("yyyy-MM-dd"));
        }
        userData.User.Sex.Should().Be(user.Sex);
        userData.User.Gender.Should().Be(user.Gender);
    }

    protected void ShouldBeEquivalentTo(IList<Users.API.Models.Shared.UserData> embedded, Users.Domain.Models.User user)
    {
        var users = new List<Users.Domain.Models.User> { user };
        ShouldBeEquivalentTo(embedded, users);
    }

    protected void ShouldBeEquivalentTo(IList<Users.API.Models.Shared.UserData> embedded, IList<Users.Domain.Models.User> users)
    {
        foreach (var userData in embedded)
        {
            userData.Should().NotBeNull();
            var embeddedUser = userData.User;
            var userId = embeddedUser.Id;
            var user = users.Single(_ => _.Id == userId);
            user.Should().NotBeNull();
            ShouldBeEquivalentTo(userData, user);
            //ShoudBeEquivalentTo(userData, user);
            //        ShouldBeCorrect(userData.Links, userId);
        }
    }

    //private void ShoudBeEquivalentTo(Users.API.Models.Shared.UserData userData, Users.Domain.Models.User user)
    //{
    //    throw new NotImplementedException();
    //}

    //protected void ShouldBeEquivalentTo(IEnumerable<Users.API.Models.Shared.UserData> embedded, Users.Domain.Models.User user)
    //{
    //    var users = new List<Users.Domain.Models.User> { user };
    //    ShoudBeEquivalentTox(embedded, users);
    //}

    //protected void ShouldBeEquivalentTo(IEnumerable<Users.API.Models.Shared.UserData> embedded, IList<Users.Domain.Models.User> users)
    //{
    //    foreach (var userData in embedded)
    //    {
    //        var embeddedUser = userData.User;
    //        var userId = embeddedUser.Id;
    //        var user = users.SingleOrDefault(_ => _.Id == userId);
    //        //ShoudBeEquivalentTo(userData, user);
    //        ShouldBeCorrect(userData.Links, userId);
    //    }
    //}

    //protected void ShoudBeEquivalentTo(Users.API.Models.Shared.UserData userData, Users.Domain.Models.User user)
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