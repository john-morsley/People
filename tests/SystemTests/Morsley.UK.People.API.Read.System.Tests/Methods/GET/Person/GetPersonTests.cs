namespace Morsley.UK.People.API.Read.System.Tests.Methods.GET.Person;

public class GetPersonTests : ReadApplicationTestFixture<ReadProgram>
{
    [Test]
    [Category("Happy")]
    public async Task Given_Person_Exists___When_That_Person_Is_Requested___Then_200_OK_And_Person_Returned()
    {
        // Arrange...
        ReadDatabase.NumberOfPeople().Should().Be(0);

        var expected = ReadDatabase.GeneratePerson();
        await ReadDatabase.AddPerson(expected);

        ReadDatabase.NumberOfPeople().Should().Be(1);

        var url = $"/api/person/{expected.Id}";

        await AuthenticateAsync(Username, Password);

        // Act...
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        ReadDatabase.NumberOfPeople().Should().Be(1);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
        var userResource = DeserializePersonResource(content);

        // - User
        userResource.Should().NotBeNull();
        userResource!.Data.Should().NotBeNull();
        ShouldBeEquivalentTo(userResource, expected);

        // - Links
        userResource!.Links.Should().NotBeNull();
        userResource!.Links!.Count.Should().Be(3);
        LinksForPersonShouldBeCorrect(userResource.Links, expected.Id);

        // - Embedded
        userResource!.Embedded.Should().BeNull();
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Person_Does_Not_Exist___When_That_Person_Is_Requested___Then_204_NoContent()
    {
        // Arrange...
        ReadDatabase.NumberOfPeople().Should().Be(0);

        var personId = Guid.NewGuid();
        var url = $"/api/person/{personId}";

        await AuthenticateAsync(Username, Password);

        // Act...
        var result = await HttpClient!.GetAsync(url);
        //var result = await HttpClient!.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));

        // Assert...
        ReadDatabase.NumberOfPeople().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_User_Is_Requested_With_Invalid_Id___Then_404_NotFound()
    {
        // Arrange...
        ReadDatabase.NumberOfPeople().Should().Be(0);

        const string url = "/api/person/invalid-user-id";

        await AuthenticateAsync(Username, Password);

        // Act...
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        ReadDatabase.NumberOfPeople().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
    }
}