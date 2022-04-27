namespace Morsley.UK.People.API.Read.System.Tests.Methods.GET.People;

public class GetPeopleWithFilterTests : ReadApplicationTestFixture<ReadProgram>
{
    private const string UserDataForFilter = "Mark Pink|" +
                                             "John Green|" +
                                             "Jane Doe|" +
                                             "John Doe|" +
                                             "Tom Yellow|" +
                                             "Jane Brown";

    private const string UserDataForFilterWithAllSexes = "Dave Pink|" +
                                                         "Peter Brown,Sex:Intersex|" +
                                                         "Jane Doe,Sex:Female|" +
                                                         "John Doe,Sex:Male|" +
                                                         "Mary Green|" +
                                                         "Jane White,Sex:PreferNotToSay";

    private const string UserDataForFilterWithAllGenders = "Peter Brown,Gender:Bigender|" +
                                                           "Lisa Yellow,Gender:Cisgender|" +
                                                           "Jane Doe|" +
                                                           "John Doe|" +
                                                           "Mary Green,Gender:GenderFluid|" +
                                                           "Karen Blue,Gender:Transgender|" +
                                                           "Mary Red,Gender:Agender|" +
                                                           "Jane White,Gender:PreferNotToSay|" +
                                                           "Mark Orange,Gender:NonBinary";

    private const string UserDataForFilterWithDateOfBirth = "James Red,DateOfBirth:2000-01-31|" +
                                                            "Mary Orange|" +
                                                            "Robert Yellow,DateOfBirth:1998-10-01|" +
                                                            "Susan Green|" +
                                                            "David Blue,DateOfBirth:1998-12-05|" +
                                                            "Lisa Indigo,DateOfBirth:1998-12-05|" +
                                                            "Andy Violet,DateOfBirth:1990-03-21";

    [Test]
    [Category("Happy")]
    [TestCase("LastName:Doe", UserDataForFilter, "Jane Doe|John Doe")]
    [TestCase("FirstName:John", UserDataForFilter, "John Green|John Doe")]
    [TestCase("Sex", UserDataForFilterWithAllSexes, "Dave Pink|Mary Green")]
    [TestCase("Sex:Male", UserDataForFilterWithAllSexes, "John Doe")]
    [TestCase("Sex:Female", UserDataForFilterWithAllSexes, "Jane Doe")]
    [TestCase("Gender", UserDataForFilterWithAllGenders, "Jane Doe|John Doe")]
    [TestCase("Gender:Cisgender", UserDataForFilterWithAllGenders, "Lisa Yellow")]
    [TestCase("DateOfBirth", UserDataForFilterWithDateOfBirth, "Mary Orange|Susan Green")]
    [TestCase("DateOfBirth:1998-12-05", UserDataForFilterWithDateOfBirth, "David Blue|Lisa Indigo")]
    public async Task Given_People_Exist___When_People_Are_Requested_With_Valid_Filter___Then_200_OK_And_Filtered_People_Returned(
        string validFilter, 
        string testPeopleData, 
        string expectedPeople)
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        ApplicationReadDatabase.NumberOfPeople().Should().Be(0);
        var people = ApplicationReadDatabase.AddTestPeopleToDatabase(testPeopleData);
        ApplicationReadDatabase.NumberOfPeople().Should().Be(people.Count);

        var url = $"/api/people?filter={validFilter}";

        await AuthenticateAsync(Username, Password);

        // Act...
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

        // - Embedded
        personData.Embedded.Should().NotBeNull();
        personData.Embedded!.Count.Should().Be(expectedPeople.Split('|').Length);
        foreach(var name in expectedPeople.Split('|'))
        {
            var kvp = name.Split(' ');
            var firstName = kvp[0];
            var lastName = kvp[1];
            var expectedPerson = people.Single(_ => _.FirstName == firstName && _.LastName == lastName);
            var actualPersonData = personData.Embedded.Single(_ => _.Data!.FirstName == firstName && _.Data.LastName == lastName);
            actualPersonData.Should().NotBeNull();

            ShouldBeEquivalentTo(actualPersonData, expectedPerson);

            // User...
            //actualUserData.Data.Should().NotBeNull();
            //actualUserData.Data!.Id.Should().Be(expectedUser.Id);
            //if (actualUserData.Data.Sex is null && expectedUser.Sex is null)
            //{
            //    actualUserData.Data.Sex.Should().BeNull();
            //    expectedUser.Sex.Should().BeNull();
            //}
            //else if (actualUserData.Data.Sex is null || expectedUser.Sex is null)
            //{
            //    // This should never happen...
            //    Assert.Fail("Sexes should always be equal!");
            //}
            //else
            //{
            //    actualUserData.Data.Sex.Should().Be(expectedUser.Sex.ToString());    
            //}
            //if (actualUserData.Data.Gender is null && expectedUser.Gender is null)
            //{
            //    actualUserData.Data.Gender.Should().BeNull();
            //    expectedUser.Gender.Should().BeNull();
            //}
            //else if (actualUserData.Data.Gender is null || expectedUser.Gender is null)
            //{
            //    // This should never happen...
            //    Assert.Fail("Genders should always be equal!");
            //}
            //else
            //{
            //    actualUserData.Data.Gender.Should().Be(expectedUser.Gender.ToString());
            //}

            // Links...
            actualPersonData.Links.Should().NotBeNull();
            actualPersonData.Links!.Count.Should().Be(3);
            LinksForPersonShouldBeCorrect(actualPersonData.Links, actualPersonData.Data.Id);

            // Embedded...
            actualPersonData.Embedded.Should().BeNull();
            
            //actualUser.DateOfBirth.Should().Be(expectedUser.DateOfBirth.InternationalFormat());
        }

        // - Links
        personData.Links.Should().NotBeNull();
        personData!.Links.Count.Should().Be(1);
        LinksForPeopleShouldBeCorrect(personData.Links, pageNumber, pageSize, filter: validFilter, totalNumber: people.Count);
    }

    // ToDo --> Broken by ASP.NET 6 and Minimal API validation.
    [Test]
    [Category("Unhappy")]
    [TestCase("Id:ShouldNotBeUsed")]
    [TestCase("InvalidFieldName")]
    public async Task When_A_Page_Of_People_Is_Requested_With_Invalid_Filter___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues(string invalidFilter)
    {
        // Arrange...
        ApplicationReadDatabase.NumberOfPeople().Should().Be(0);

        var url = $"/api/people?filter={invalidFilter}";

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
            { "Filter", "The filter value is invalid. e.g. filter=sex:male" }
        };
        ProblemDetailsShouldContainValidationIssues(content, validationErrors);

        //var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(response);
        //problemDetails.Should().NotBeNull();
        //problemDetails.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        //problemDetails.Title.Should().Be("Validation error(s) occurred!");
        //problemDetails.Detail.Should().Be("See the errors field for details.");
        //problemDetails.Extensions.Should().NotBeNull();
        //problemDetails.Extensions.Count().Should().Be(1);
        //var issue = problemDetails.Extensions.First();
        //issue.Key.Should().Be("Field");
        //var value = issue.Value!.ToString();
        //value.Should().Be("The filter value is invalid. e.g. filter=sex:male");
    }
}