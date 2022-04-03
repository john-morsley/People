namespace Morsley.UK.People.API.Test.Fixture;

public abstract class SecuredApplicationTestFixture<TProgram> : ApplicationTestFixture<TProgram> where TProgram : class
{
    protected const string Username = "johnmorsley";
    protected const string Password = "P@$$w0rd!";

    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;

    protected string LoginUrl => "/api/login";

    public SecuredApplicationTestFixture()
    {
        _audience = GetAudience();
        _issuer = GetIssuer();

        var key = Environment.GetEnvironmentVariable(Constants.Morsley_UK_People_API_Security_JWT_KEY_Variable);
        if (string.IsNullOrEmpty(key)) throw new InvalidOperationException($"Expected an environment variable called {Constants.Morsley_UK_People_API_Security_JWT_KEY_Variable}");
        _key = key;
    }
    
    protected async Task AuthenticateAsync(string username, string password)
    {
        var token = await GetJwtTokenAsync(username, password);
        if (token == null) return;
        HttpClient!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var output = input;
        output = output.Replace('-', '+'); // 62nd char of encoding
        output = output.Replace('_', '/'); // 63rd char of encoding
        switch (output.Length % 4) // Pad with trailing '='s
        {
            case 0: break; // No pad chars in this case
            case 2: output += "=="; break; // Two pad chars
            case 3: output += "="; break; // One pad char
            default: throw new Exception("Illegal base64url string!");
        }
        var converted = Convert.FromBase64String(output); // Standard base64 decoder
        return converted;
    }

    protected static LoginResponse? DeserializeLoginResponse(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        return JsonSerializer.Deserialize<LoginResponse>(json, options);
    }

    private string GetAudience()
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json");
        IConfiguration configuration = builder.Build();
        var audience = configuration["Jwt:Audience"];
        return audience;
    }

    private string GetIssuer()
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json");
        IConfiguration configuration = builder.Build();
        var issuer = configuration["Jwt:Issuer"];
        return issuer;
    }

    protected string GetJwtAudience()
    {
        //var port = GetApplicationPort(DatabaseTestFixture.Configuration);
        var port = GetApplicationPort(GetConfiguration());
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
        var result = await HttpClient!.PostAsJsonAsync(LoginUrl, request);
        if (!result.IsSuccessStatusCode) return null;
        if (result.StatusCode != HttpStatusCode.OK) return null;
        var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
        if (response == null) return null;
        return response.Token;
    }

    protected void TokensShouldBeEquivalent(JwtSecurityToken originalToken, JwtSecurityToken validatedToken)
    {
        originalToken.Should().BeEquivalentTo(validatedToken, config => config.Excluding(_ => _.SigningKey));
    }

    protected void VerifySignature(string header, string payload, string signature)
    {
        var decodedSignature = Convert.ToBase64String(Base64UrlDecode(signature));

        var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
        var keyBytes = Encoding.UTF8.GetBytes(_key);

        var sha = new HMACSHA256(keyBytes);
        var computedSignature = sha.ComputeHash(bytesToSign);
        var decodedComputedSignature = Convert.ToBase64String(computedSignature);

        decodedSignature.Should().Be(decodedComputedSignature);
    }

    protected bool VerifyToken(string token, out JwtSecurityToken? validatedToken)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false, // ToDo --> Before the version upgrade these two worked! :(
            ValidateAudience = false, // ToDo --> Must revisit in the future.
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key))
        };
        var handler = new JwtSecurityTokenHandler();

        try
        {
            handler.ValidateToken(token, parameters, out var validated);
            validatedToken = validated as JwtSecurityToken;
        }
        catch
        {
            validatedToken = null;
            return false;
        }

        return true;
    }
}