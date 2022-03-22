namespace Morsley.UK.People.API.Read.System.Tests.Methods;

public class OPTIONS : ApplicationTestFixture<ReadProgram>
{
    [Test]
    [Category("Happy")]
    public async Task Given_An_API_With_Unknown_Capabilities___When_An_Options_Call_Is_Made_For_People___Then_200_OK_And_Allowed_HTTP_Methods_Are_Returned()
    {
        // Arrange...
        var expectedOptions = new List<string> { "GET", "HEAD", "OPTIONS" };
        const string url = "/api/people/";

        // Act...
        var message = new HttpRequestMessage(HttpMethod.Options, url);
        var result = await HttpClient!.SendAsync(message);

        // Assert...
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Content.Headers.Allow.Count.Should().Be(3);
        result.Content.Headers.Allow.Should().BeEquivalentTo(expectedOptions);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_An_API_With_Unknown_Capabilities___When_An_Options_Call_Is_Made_For_Person___Then_200_OK_And_Allowed_HTTP_Methods_Are_Returned()
    {
        // Arrange...
        var expectedOptions = new List<string> { "GET", "HEAD", "OPTIONS" };
        const string url = "/api/person/";

        // Act...
        var message = new HttpRequestMessage(HttpMethod.Options, url);
        var result = await HttpClient!.SendAsync(message);

        // Assert...
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Content.Headers.Allow.Count.Should().Be(3);
        result.Content.Headers.Allow.Should().BeEquivalentTo(expectedOptions);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

    [TearDown]
    public void TearDown()
    {
        HttpClient!.Dispose();
    }
}