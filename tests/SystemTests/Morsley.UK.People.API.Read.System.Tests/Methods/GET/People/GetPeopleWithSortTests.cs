namespace Morsley.UK.People.API.Read.System.Tests.Methods.GET.People;

public class GetPeopleWithSortTests : ReadApplicationTestFixture<ReadProgram>
{
    private const string TestUserData = "Mark Pink|" +
                                        "George Boole,Sex:Male,DateOfBirth:1815-11-02|" +
                                        "John Cleese,Sex:Male,DateOfBirth:1939-10-27|" +
                                        "Jane Doe,Sex:Female|" +
                                        "John Doe,Sex:Male|" +
                                        "Tommy Flowers,Sex:Male,DateOfBirth:1905-12-22|" +
                                        "Jane Goodall,Sex:Female,DateOfBirth:1934-04-03|" +
                                        "Ada Lovelace,Sex:Female,DateOfBirth:1815-12-10|" +
                                        "Linus Torvalds,Sex:Male,DateOfBirth:1969-12-28|" +
                                        "Alan Turing";

    private const string FirstNameAscLastNameAsc = "Ada Lovelace|Alan Turing|George Boole|Jane Doe|Jane Goodall|John Cleese|John Doe|Linus Torvalds|Mark Pink|Tommy Flowers";

    private const string LastNameAscFirstNameAsc = "George Boole|John Cleese|Jane Doe|John Doe|Tommy Flowers|Jane Goodall|Ada Lovelace|Mark Pink|Linus Torvalds|Alan Turing";

    private const string FirstNameDescLastNameDesc = "Tommy Flowers|Mark Pink|Linus Torvalds|John Doe|John Cleese|Jane Goodall|Jane Doe|George Boole|Alan Turing|Ada Lovelace";

    private const string LastNameDescFirstNameDesc = "Alan Turing|Linus Torvalds|Mark Pink|Ada Lovelace|Jane Goodall|Tommy Flowers|John Doe|Jane Doe|John Cleese|George Boole";

    private const string DateOfBirthAsc = "[Unknown]|[Unknown]|[Unknown]|[Unknown]|George Boole|Ada Lovelace|Tommy Flowers|Jane Goodall|John Cleese|Linus Torvalds";

    private const string DateOfBirthDesc = "Linus Torvalds|John Cleese|Jane Goodall|Tommy Flowers|Ada Lovelace|George Boole|[Unknown]|[Unknown]|[Unknown]|[Unknown]";

    [Test]
    [Category("Happy")]
    [TestCase("FirstName:asc,LastName:asc", TestUserData, FirstNameAscLastNameAsc)]
    [TestCase("LastName:asc,FirstName:asc", TestUserData, LastNameAscFirstNameAsc)]
    [TestCase("FirstName:desc,LastName:desc", TestUserData, FirstNameDescLastNameDesc)]
    [TestCase("LastName:desc,FirstName:desc", TestUserData, LastNameDescFirstNameDesc)]
    [TestCase("DateOfBirth:asc", TestUserData, DateOfBirthAsc)]
    [TestCase("DateOfBirth:desc", TestUserData, DateOfBirthDesc)]
    public async Task Given_People_Exist___When_A_Page_Of_People_Is_Requested_With_Valid_Sort___Then_200_OK_And_People_Should_Be_Sorted(
        string validSort, 
        string testPeopleData, 
        string expectedOrder)
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        ApplicationReadDatabase.NumberOfPeople().Should().Be(0);
        var people = ApplicationReadDatabase.AddPeopleToDatabase(testPeopleData);
        ApplicationReadDatabase.NumberOfPeople().Should().Be(people.Count);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/people?sort={validSort}";
        var response = await HttpClient!.GetAsync(url);

        // Assert...
        ApplicationReadDatabase.NumberOfPeople().Should().Be(people.Count);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var personData = DeserializePersonResource(content);
        personData.Should().NotBeNull();

        // - User
        personData!.Data.Should().BeNull();

        // - Links
        personData.Links.Should().NotBeNull();
        personData!.Links!.Count.Should().Be(1);
        LinksForPeopleShouldBeCorrect(personData.Links, pageNumber, pageSize, sort: validSort, totalNumber: people.Count);

        // - Embedded
        var index = 0;
        foreach (var name in expectedOrder.Split('|'))
        {
            var actualPersonData = personData.Embedded!.Skip(index).Take(1).Single();
            index++;

            if (name == "[Unknown]") continue;

            var kvp = name.Split(' ');
            var firstName = kvp[0];
            var lastName = kvp[1];
            var expectedPerson = people.Single(_ => _.FirstName == firstName && _.LastName == lastName);

            // - User
            ShouldBeEquivalentTo(actualPersonData, expectedPerson);

            // Links...
            actualPersonData.Links.Should().NotBeNull();
            actualPersonData.Links!.Count.Should().Be(3);
            LinksForPersonShouldBeCorrect(actualPersonData.Links, actualPersonData.Data.Id);

            // Embedded...
            actualPersonData.Embedded.Should().BeNull();
        }
    }

    [Test]
    [Category("Unhappy")]
    [TestCase("Id")]
    [TestCase("InvalidFieldName")] 
    public async Task When_A_Page_Of_People_Is_Requested_With_Invalid_Sorting___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues(
        string invalidSort)
    {
        // Arrange...
        ApplicationReadDatabase.NumberOfPeople().Should().Be(0);

        var url = $"/api/people?sort={invalidSort}";

        await AuthenticateAsync(Username, Password);

        // Act...
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        ApplicationReadDatabase.NumberOfPeople().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var validationErrors = new Dictionary<string, string>
        {
            { "Sort", $"The sort value is invalid. e.g. sort={Morsley.UK.People.API.Contracts.Constants.Defaults.DefaultPageSort}" }
        };
        ProblemDetailsShouldContainValidationIssues(content, validationErrors);
    }
}