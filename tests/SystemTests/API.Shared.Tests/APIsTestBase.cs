using NUnit.Framework.Internal;
using System.Web;

namespace API.Shared.Tests;

public class APIsTestBase<TStartUp> : TestBase where TStartUp : class
{
    protected const string API_MEDIA_TYPE = "application/json";

    protected MongoContext _mongoContext;

    private TestServer _server;
    protected HttpClient _client;

    [SetUp]
    public void Setup()
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
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }

    //public static Users.API.Models.Response.v1.UserResponse DeserializeUser(string json)
    //{
    //    var options = new JsonSerializerOptions
    //    {
    //        PropertyNameCaseInsensitive = true,
    //        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    //    };

    //    return JsonSerializer.Deserialize<Users.API.Models.Response.v1.UserResponse>(json, options);
    //}

    //public static Users.API.Models.Shared.Metadata DeserializeMetadata(string json)
    //{
    //    var options = new JsonSerializerOptions
    //    {
    //        PropertyNameCaseInsensitive = true
    //    };
    //    return JsonSerializer.Deserialize<Users.API.Models.Shared.Metadata>(json, options);
    //}

    //public static Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse> DeserializePagedListOfUserResponses(string json)
    //{
    //    var options = new JsonSerializerOptions()
    //    {
    //        PropertyNameCaseInsensitive = true
    //    };
    //    options.Converters.Add(new Users.API.Models.Shared.PagedListJsonConverter());
    //    return JsonSerializer.Deserialize<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>(json, options);
    //}

    //public static IEnumerable<Users.API.Models.Response.v1.UserResponse> DeserializeEmbeddedUsers(string json)
    //{
    //    var options = new JsonSerializerOptions()
    //    {
    //        PropertyNameCaseInsensitive = true,
    //        Converters =
    //        {
    //            new Users.API.Models.Shared.EmbeddedUsersConverter(),
    //            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
    //        }
    //    };

    //    return JsonSerializer.Deserialize<IEnumerable<Users.API.Models.Response.v1.UserResponse>>(json, options);
    //}

    protected static string BuildExpectedUrl(int pageNumber, int pageSize, string? filter, string? search, string? sort)
    {
        const string baseUrl = "http://localhost/api/v1/users";

        var filterParameter = "";
        if (filter != null) filterParameter = $"&filter={filter}";

        var searchParameter = "";
        if (search != null) searchParameter = $"&search={search}";

        var sortParameter = "";
        if (sort == null)
        {
            sortParameter = $"&sort={Users.API.Models.Constants.Defaults.DefaultPageSort}";
        }
        else
        {
            sortParameter = $"&sort={sort}";
        }

        var expectedUrl = $"{baseUrl}" +
                          $"?pageNumber={pageNumber}&pageSize={pageSize}" +
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

    //public static IEnumerable<Users.API.Models.Response.v1.UserResponse> DeserializePageOfUsers(string json)
    //{
    //    var options = new JsonSerializerOptions()
    //    {
    //        PropertyNameCaseInsensitive = true,
    //        Converters =
    //        {
    //            new Users.API.Models.Shared.PagedListJsonConverter(),
    //            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
    //        }
    //    };
        
    //    return JsonSerializer.Deserialize<IEnumerable<Users.API.Models.Response.v1.UserResponse>>(json, options);
    //}

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

    protected void LinksForPageOfUsersShouldBeCorrect(
        IList<Users.API.Models.Shared.Link> links, 
        int pageNumber, 
        int pageSize, 
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
        var expectedUrl = BuildExpectedUrl(pageNumber, pageSize, filter, search, sort);

        var currentPageOfUsersUrl = HttpUtility.UrlDecode(currentPageOfUsersLink.HypertextReference);
        currentPageOfUsersUrl.Should().Be(expectedUrl);

        // Next page...

    }

    protected void LinksForUserShouldBeCorrect(IList<Users.API.Models.Shared.Link> links, Guid userId)
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