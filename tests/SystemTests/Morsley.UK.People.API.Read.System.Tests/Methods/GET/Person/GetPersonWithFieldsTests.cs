using Morsley.UK.People.API.Contracts.Responses;

namespace Morsley.UK.People.API.Read.System.Tests.Methods.GET.Person;

public class GetPersonWithFieldsTests : ReadApplicationTestFixture<ReadProgram>
{
    [Category("Happy")]
    [Test]    
    [TestCase("FirstName")]
    [TestCase("LastName")]
    [TestCase("Sex")]
    [TestCase("Gender")]
    [TestCase("DateOfBirth")]
    [TestCase("FirstName,LastName,Sex,Gender,DateOfBirth")]
    public async Task Given_Person_Exist___When_It_Is_Requested_With_Fields___Then_200_OK_And_Person_Should_Be_Shaped(string validFields)
    {
        // Arrange...
        ReadDatabase.NumberOfPeople().Should().Be(0);

        var user = ReadDatabase.GeneratePerson();
        ReadDatabase.AddPersonToDatabase(user);

        ReadDatabase.NumberOfPeople().Should().Be(1);

        var url = $"/api/person/{user.Id}?fields={validFields}";

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
        userResource.Should().NotBeNull();

        validFields = AddToFieldsIfMissing("Id", validFields);
        var (expected, unexpected) = DetermineExpectedAndUnexpectedFields(validFields);

        // - Person
        userResource!.Data.Should().NotBeNull();
        ShouldBeEquivalent(expected, userResource.Data, user);
        ShouldBeNull(unexpected, userResource.Data);

        // - Links
        userResource.Links.Should().NotBeNull();
        userResource.Links!.Count.Should().Be(3);
        LinksForPersonShouldBeCorrect(userResource.Links, user.Id);

        // - Embedded
        userResource.Embedded.Should().BeNull();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <notes>
    /// As it appears that ASP.NET 6 has broken automatic validation, we cannot have a 422 here like we'd prefer.
    /// So for now, we are going to have to do with a 204.
    /// ToDo --> We really need to work out a way to return the proper problem details like before.
    /// </notes>
    [Test]
    [Category("UnHappy")]
    public async Task When_A_Person_Is_Requested_With_Invalid_Fields___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        ReadDatabase.NumberOfPeople().Should().Be(0);

        var personId = Guid.NewGuid();
        var url = $"/api/person/{personId}?fields=fielddoesnotexist";

        await AuthenticateAsync(Username, Password);

        // Act...
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        ReadDatabase.NumberOfPeople().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var validationErrors = new Dictionary<string, string>
        {
            { "Fields", "The fields value is invalid. e.g. fields=id,lastname" }
        };
        ProblemDetailsShouldContainValidationIssues(content, validationErrors);
    }

    private static void ShouldBeEquivalent(IList<string> fieldNames, PersonResponse actual, Domain.Models.Person expected)
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

    private static void ShouldBeNull(IList<string> fieldNames, PersonResponse actual)
    {
        foreach (var fieldName in fieldNames)
        {
            ShouldBeNull(fieldName, actual);
        }
    }

    private static void ShouldBeNull(string fieldName, PersonResponse actual)
    {
        var actualValue = GetValue(actual, fieldName);
        actualValue.Should().BeNull();
    }
}