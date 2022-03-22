namespace Morsley.UK.People.API.Test.Fixture;

public abstract class SecuredApplicationTestFixture<TProgram, TSecurityProgram> : ApplicationTestFixture<TProgram>
    where TProgram : class
    where TSecurityProgram : class
{
    //protected const string SecurityUrl = "https://localhost:5000";

    protected const string Username = "johnmorsley";
    protected const string Password = "P@$$w0rd!";

    protected HttpClient? SecurityClient;

    private int _securityPort;

    protected int SecurityPort
    {
        get
        {
            if (_securityPort == 0) _securityPort = GetSecurityPort(Configuration);
            return _securityPort;
        }
    }

    [SetUp]
    protected override void SetUp()
    {
        var factory = new WebApplicationFactory<TSecurityProgram>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseUrls($"https://localhost:{SecurityPort}");
                builder.UseKestrel();
                builder.ConfigureKestrel(options =>
                    {
                        options.ListenLocalhost(SecurityPort);
                    }
                );
                builder.ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddConfiguration(Configuration);
                    //configuration.AddInMemoryCollection(GetInMemoryConfiguration());
                });
                //builder.ConfigureServices();
            });
        SecurityClient = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            BaseAddress = new System.Uri($"https://localhost:{SecurityPort}")
        });

        base.SetUp();
    }

    [TearDown]
    protected override void TearDown()
    {
        base.TearDown();
        SecurityClient?.Dispose();
    }

    protected async Task AuthenticateAsync(string username, string password)
    {
        var token = await GetJwtTokenAsync(username, password);
        if (token == null) return;
        HttpClient!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
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
    //    var additional = GetInMemoryConfiguration();
    //    var configuration = GetConfiguration(additional);
    //    return configuration;
    //}

    protected override Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = base.GetInMemoryConfiguration();
        additional.Add("Jwt:Key", GetJwtKey());
        additional.Add("Jwt:Audience", GetJwtAudience());
        return additional;
    }

    protected string GetJwtAudience()
    {
        var port = GetApplicationPort(Configuration);
        var key = $"https://localhost:{port}";
        if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException($"Missing environment variable: {Constants.Morsley_UK_People_API_Security_JWT_KEY_Variable}");
        return key;
    }

    protected string GetJwtKey()
    {
        var key = Environment.GetEnvironmentVariable(Constants.Morsley_UK_People_API_Security_JWT_KEY_Variable);
        if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException($"Missing environment variable: {Constants.Morsley_UK_People_API_Security_JWT_KEY_Variable}");
        return key;
    }

    protected async Task<string?> GetJwtTokenAsync(string username, string password)
    {
        Log.Logger.Information("GetJwtTokenAsync");

        var request = new LoginRequest(username, password);
        var result = await SecurityClient!.PostAsJsonAsync(LoginUrl, request);
        if (!result.IsSuccessStatusCode) return null;
        if (result.StatusCode != HttpStatusCode.OK) return null;
        var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
        if (response == null) return null;
        return response.Token;
    }

    protected int GetSecurityPort(IConfiguration configuration)
    {
        var potentialPort = configuration["SecurityPort"];

        if (string.IsNullOrEmpty(potentialPort)) throw new InvalidProgramException("Invalid configuration --> Port is missing!");

        if (int.TryParse(potentialPort, out var port)) return port;

        throw new InvalidProgramException("Invalid configuration --> port is not a number!");
    }

    //protected string LoginUrl => $"{SecurityUrl}/login";
    protected string LoginUrl => "/login";
}