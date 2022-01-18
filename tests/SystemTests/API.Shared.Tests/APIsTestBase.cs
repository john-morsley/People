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

    public static Users.API.Models.Response.v1.UserResponse DeserializeUserResponse(string json)
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<Users.API.Models.Response.v1.UserResponse>(json, options);
    }

    public static Users.API.Models.Shared.HATEOAS DeserializeHATEOAS(string json)
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<Users.API.Models.Shared.HATEOAS>(json, options);
    }

    //public static Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse> DeserializePagedListOfUserResponses(string json)
    //{
    //    var options = new JsonSerializerOptions()
    //    {
    //        PropertyNameCaseInsensitive = true
    //    };
    //    options.Converters.Add(new Users.API.Models.Shared.PagedListJsonConverter());
    //    return JsonSerializer.Deserialize<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>(json, options);
    //}

    public static IEnumerable<Users.API.Models.Response.v1.UserResponse> DeserializeListOfUserResponses(string json)
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        //options.Converters.Add(new Users.API.Models.Shared.PagedListJsonConverter());
        return JsonSerializer.Deserialize<IEnumerable<Users.API.Models.Response.v1.UserResponse>>(json, options);
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
}
