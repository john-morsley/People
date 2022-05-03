namespace Morsley.UK.People.API.Read.System.Tests.Methods.HEAD.Person;

public class HeadPersonTests : ReadApplicationTestFixture<ReadProgram>
{
    [Test]
    [Category("Happy")]
    public async Task Given_No_People_Exist___When_People_Are_Requested___Then_204_NoContent()
    {
        // Arrange...
        ApplicationReadDatabase!.NumberOfPeople().Should().Be(0);
        var personId = Guid.NewGuid();

        var url = $"/api/person/{personId}";

        await AuthenticateAsync(Username, Password);

        var request = new HttpRequestMessage(HttpMethod.Head, url);

        // Act...
        var result = await HttpClient!.SendAsync(request);

        // Assert...
        ApplicationReadDatabase!.NumberOfPeople().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Content.Headers.ContentLength.Should().BeNull();

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_People_Exists___When_People_Are_Requested___Then_200_OK_And_Content_Length_Returned()
    {
        // Arrange...
        ApplicationReadDatabase!.NumberOfPeople().Should().Be(0);
        var user = ApplicationReadDatabase.GenerateTestPerson();
        ApplicationReadDatabase!.AddPersonToDatabase(user);
        ApplicationReadDatabase!.NumberOfPeople().Should().Be(1);

        const string url = "/api/person?PageNumber=1&PageSize=10";

        await AuthenticateAsync(Username, Password);

        var expectedContentLength = await DetermineExpectedContentLength(url);

        var request = new HttpRequestMessage(HttpMethod.Head, url);

        // Act...
        var result = await HttpClient!.SendAsync(request);

        // Assert...
        ApplicationReadDatabase!.NumberOfPeople().Should().Be(1);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Content.Headers.ContentLength.Should().Be(expectedContentLength);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }
}