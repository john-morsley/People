namespace Morsley.UK.People.API.Write.Tests.Methods;

public class POST_Login : WriteApplicationTestFixture<WriteProgram>
{
    [Test]
    public async Task Trying_To_Authenticate___With_Invalid_Credentials___Should_Result_In_401_Unauthorized()
    {
        // Arrange...
        const string url = "https://localhost/login";

        var request = new LoginRequest("invalid", "invalid");

        // Act...
        var result = await HttpClient!.PostAsJsonAsync(url, request);

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
        var result = await HttpClient!.PostAsJsonAsync(url, request);

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
}
