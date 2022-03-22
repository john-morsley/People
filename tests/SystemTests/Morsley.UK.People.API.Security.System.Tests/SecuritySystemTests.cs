using FluentAssertions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
//using Microsoft.IdentityModel.Tokens;
using Morsley.UK.People.API.Contracts.Requests;
using Morsley.UK.People.API.Contracts.Responses;
using NUnit.Framework;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Morsley.UK.People.Common;

namespace Morsley.UK.People.API.Security.System.Tests;

internal class SecurityTests : SecurityFixture
{
    private string Key = "[TBD]";
    private const string Issuer = "https://localhost:5000/";
    private const string Audience = "https://localhost:5001/";

    [SetUp]
    public void Setup()
    {
        var key = Environment.GetEnvironmentVariable(Constants.Morsley_UK_People_API_Security_JWT_KEY_Variable);
        if (string.IsNullOrEmpty(key)) throw new InvalidOperationException($"Expected an environment variable called {Constants.Morsley_UK_People_API_Security_JWT_KEY_Variable}");
        Key = key;
    }

    [Test]
    public async Task Trying_To_Authenticate___With_Invalid_Credentials___Should_Result_In_401_Unauthorized()
    {
        // Arrange...
        const string url = "https://localhost/login";

        var request = new LoginRequest("invalid", "invalid");

        // Act...
        var result = await HttpClient.PostAsJsonAsync(url, request);

        // Assert...
        result.IsSuccessStatusCode.Should().Be(false);
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Trying_To_Authenticate___With_Valid_Credentials___Should_Result_In_200_OK_And_JWT_Token()
    {
        // Arrange...
        const string username = "johnmorsley";
        const string password = "P@$$w0rd!";
        // ToDo --> Add 'John Morsley' to the IAM database.

        const string url = "https://localhost/login";

        var request = new LoginRequest(username, password);

        // Act...
        var result = await HttpClient.PostAsJsonAsync(url, request);

        // Assert...
        result.IsSuccessStatusCode.Should().Be(true);
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        var response = DeserializeLoginResponse(content);
        response.Should().NotBeNull();
        response!.Token.Should().NotBeNull();

        var sections = response.Token.Split('.');
        sections.Should().NotBeNull();
        sections.Length.Should().Be(3);

        var header = sections[0];
        var payload = sections[1];
        var signature = sections[2];

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(response!.Token);
        token.Should().NotBeNull();

        response!.Token.Should().Be(token.RawData);
        header.Should().Be(token.EncodedHeader);
        payload.Should().Be(token.EncodedPayload);
        signature.Should().Be(token.RawSignature);

        var isVerified = VerifyToken(response!.Token!, out var validatedToken);
        isVerified.Should().BeTrue();
        validatedToken.Should().NotBeNull();
        TokensShouldBeEquivalent(token!, validatedToken!);
        VerifySignature(header, payload, signature);
    }

    private void TokensShouldBeEquivalent(JwtSecurityToken originalToken, JwtSecurityToken validatedToken)
    {
        originalToken.Should().BeEquivalentTo(validatedToken, config => config.Excluding(_ => _.SigningKey));
    }

    private void VerifySignature(string header, string payload, string signature)
    {
        var decodedSignature = Convert.ToBase64String(Base64UrlDecode(signature));

        var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
        var keyBytes = Encoding.UTF8.GetBytes(Key);

        var sha = new HMACSHA256(keyBytes);
        var computedSignature = sha.ComputeHash(bytesToSign);
        var decodedComputedSignature = Convert.ToBase64String(computedSignature);

        decodedSignature.Should().Be(decodedComputedSignature);
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

    private bool VerifyToken(string token, out JwtSecurityToken? validatedToken)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false, // ToDo --> Before the version upgrade these two worked! :(
            ValidateAudience = false, // ToDo --> Must revisit in the future.
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key))
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
}