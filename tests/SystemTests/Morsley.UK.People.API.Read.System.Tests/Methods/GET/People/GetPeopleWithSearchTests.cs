namespace Morsley.UK.People.API.Read.System.Tests.Methods.GET.People;

public class GetPeopleWithSearchTests : ReadApplicationTestFixture<ReadProgram>
{
    private const string UserDataForSearch = "Saad Man|" +
                                             "Whet Faartz|" +
                                             "Jane Doe|" +
                                             "John Doe|" +
                                             "Tom Kuntz|" +
                                             "Tom Adamski";

    [Test]
    [Category("Happy")]
    [TestCase("Doe", UserDataForSearch, "Jane Doe|John Doe")]
    [TestCase("Tom", UserDataForSearch, "Tom Kuntz|Tom Adamski")]
    [TestCase("aa", UserDataForSearch, "Saad Man|Whet Faartz")]
    [TestCase("tz", UserDataForSearch, "Whet Faartz|Tom Kuntz")]
    public async Task Given_People_Exist___When_A_Page_Of_People_Is_Requested_With_Search_Criteria___Then_200_OK_And_People_Should_Be_Limited(string search, string peopleData, string expectedPeople)
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        ApplicationReadDatabase.NumberOfPeople().Should().Be(0);
        var numberOfExpectedPeople = UserDataForSearch.Split('|').Length;
        var people = ApplicationReadDatabase.AddPeopleToDatabase(peopleData);
        ApplicationReadDatabase.NumberOfPeople().Should().Be(numberOfExpectedPeople);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/people?search={search}";
        var response = await HttpClient!.GetAsync(url);

        // Assert...
        ApplicationReadDatabase.NumberOfPeople().Should().Be(numberOfExpectedPeople);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userData = DeserializePersonResource(content);
        userData.Should().NotBeNull();

        // User...
        userData!.Data.Should().BeNull();

        // Links...
        userData!.Links.Should().NotBeNull();
        userData!.Links!.Count.Should().Be(1);
        LinksForPeopleShouldBeCorrect(userData.Links, pageNumber, pageSize, search: search, totalNumber: numberOfExpectedPeople);

        // Embedded...
        userData.Embedded.Should().NotBeNull();
        userData!.Embedded!.Count.Should().Be(expectedPeople.Split('|').Length);
        foreach (var name in expectedPeople.Split('|'))
        {
            var kvp = name.Split(' ');
            var firstName = kvp[0];
            var lastName = kvp[1];
            var expectedPerson = people.Single(_ => _.FirstName == firstName && _.LastName == lastName);
            var actualPersonData = userData.Embedded.Single(_ => _.Data.FirstName == firstName && _.Data.LastName == lastName);
            actualPersonData.Should().NotBeNull();

            // User...
            ShouldBeEquivalentTo(actualPersonData, expectedPerson);

            // Links...
            actualPersonData.Links.Should().NotBeNull();
            actualPersonData.Links!.Count.Should().Be(3);
            LinksForPersonShouldBeCorrect(actualPersonData.Links, actualPersonData.Data.Id);

            // Embedded...
            actualPersonData.Embedded.Should().BeNull();

            //actualUser.DateOfBirth.Should().Be(expectedUser.DateOfBirth.InternationalFormat());
        }
    }
}