namespace Morsley.UK.People.API.Read.System.Tests.Methods.GET.People;

public class GetPeopleWithFieldsTests : ReadApplicationTestFixture<ReadProgram>
{    
    [Test]
    [Category("Happy")]
    [TestCase("FirstName")]
    [TestCase("LastName")]
    [TestCase("Sex")]
    [TestCase("Gender")]
    [TestCase("DateOfBirth")]
    [TestCase("FirstName,LastName,Sex,Gender,DateOfBirth")]
    public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested_With_Fields___Then_200_OK_And_Users_Should_Be_Shaped(string validFields)
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

        var user = DatabaseTestFixture.GenerateTestPerson();
        DatabaseTestFixture.AddPersonToDatabase(user);
        var users = new List<Domain.Models.Person> { user };
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(1);

        var url = $"/api/people?fields={validFields}";

        await AuthenticateAsync(Username, Password);

        // Act...
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(1);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userResource = DeserializePersonResource(content);
        userResource.Should().NotBeNull();

        var validFieldsIncludingId = AddToFieldsIfMissing("Id", validFields);
        var (expected, unexpected) = DetermineExpectedAndUnexpectedFields(validFieldsIncludingId);

        // - User
        userResource?.Data.Should().BeNull();

        // - Embedded
        userResource?.Embedded.Should().NotBeNull();
        userResource?.Embedded?.Count.Should().Be(1);
        ShouldBeEquivalent(expected, userResource?.Embedded, users);
        ShouldBeNull(unexpected, userResource?.Embedded);
        LinksForPeopleShouldBeCorrect(userResource?.Embedded);

        // - Links
        userResource?.Links.Should().NotBeNull();
        userResource?.Links?.Count.Should().Be(1);
        LinksForPeopleShouldBeCorrect(userResource?.Links, pageNumber, pageSize, fields: validFields, totalNumber: 1);
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_A_Page_Of_Users_Is_Requested_With_Invalid_Fields___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

        const string url = "/api/people?fields=fielddoesnotexist";

        await AuthenticateAsync(Username, Password);

        // Act...
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var validationErrors = new Dictionary<string, string>
        {
            { "Fields", "The fields value is invalid. e.g. fields=id,lastname" }
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
        //value.Should().Be("The fields value is invalid. e.g. fields=id,lastname");
    }

    private static void ShouldBeEquivalent(
        IEnumerable<string> fieldNames, 
        IEnumerable<PersonResource> embedded,
        IEnumerable<Domain.Models.Person> expectedUsers)
    {
        foreach (var userData in embedded)
        {
            userData.Should().NotBeNull();
            userData.Data.Should().NotBeNull();
            userData.Links.Should().NotBeNull();
            userData.Embedded.Should().BeNull();
            var expectedUser = expectedUsers.SingleOrDefault(_ => _.Id == userData.Data.Id);
            expectedUser.Should().NotBeNull();
            ShouldBeEquivalent(fieldNames, userData.Data, expectedUser);
        }
    }

    private static void ShouldBeEquivalent(
        IEnumerable<string> fieldNames,
        PersonResponse actual,
        Domain.Models.Person expected)
    {
        foreach (var fieldName in fieldNames)
        {
            ShouldBeEquivalent(fieldName, actual, expected);
        }
    }

    private static void ShouldBeEquivalent(string fieldName, PersonResponse actual, Domain.Models.Person expected)
    {
        var actualValue = GetValue(actual, fieldName);
        var expectedValue = GetValue(expected, fieldName);
        actualValue.Should().Be(expectedValue);
    }

    private static void ShouldBeNull(
        IEnumerable<string> fieldNames,
        IEnumerable<PersonResource> embedded)
    {
        foreach (var userData in embedded)
        {
            userData.Should().NotBeNull();
            userData.Data.Should().NotBeNull();
            userData.Links.Should().NotBeNull();
            userData.Embedded.Should().BeNull();
        }

        //foreach (var fieldName in fieldNames)
        //{
        //ShouldBeNull(fieldName, actual);
        //}
    }

    //private void ShouldBeNull(string fieldName, Models.Response.v1.UserResponse actual)
    //{
    //    var actualValue = GetValue(actual, fieldName);
    //    actualValue.Should().BeNull();
    //}
}