namespace Morsley.UK.People.API.Write.Tests.Methods;

public class OPTIONS : ApplicationTestFixture<WriteProgram>
{
    [Test]
    [Category("Happy")]
    public async Task Given_An_API_With_Unknown_Capabilities___When_An_Options_Call_Is_Made___Then_200_OK_And_Allowed_HTTP_Methods_Are_Returned()
    {
        // Arrange...
        var expectedOptions = new List<string> { "DELETE", "PATCH", "POST", "PUT" };
        const string url = "/api/person/";

        // Act...
        var message = new HttpRequestMessage(HttpMethod.Options, url);
        var httpResponse = await HttpClient!.SendAsync(message);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        httpResponse.Content.Headers.Allow.Count.Should().Be(4);
        httpResponse.Content.Headers.Allow.Should().BeEquivalentTo(expectedOptions);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }
}
