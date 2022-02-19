using System.Net.NetworkInformation;

namespace Users.API.Read.Tests.v1.Methods.GET.Single;

public class GetUserWithFields : APIsTestBase<StartUp>
{
    [Category("Happy")]
    [Test]    
    [TestCase("FirstName")]
    [TestCase("LastName")]
    [TestCase("Sex")]
    [TestCase("Gender")]
    [TestCase("DateOfBirth")]
    [TestCase("FirstName,LastName,Sex,Gender,DateOfBirth")]
    public async Task Given_User_Exist___When_It_Is_Requested_With_Fields___Then_200_OK_And_User_Should_Be_Shaped(string validFields)
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var userId = Guid.NewGuid();
        var expectedUser = GenerateTestUser(userId);
        AddUserToDatabase(expectedUser);

        NumberOfUsersInDatabase().Should().Be(1);

        var url = $"/api/v1/users/{userId}?fields={validFields}";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userData = DeserializeUserResource(content);
        userData.Should().NotBeNull();

        validFields = AddToFieldsIfMissing("Id", validFields);
        var (expected, unexpected) = DetermineExpectedAndUnexpectedFields(validFields);

        // - User
        userData.Data.Should().NotBeNull();
        ShouldBeEquivalent(expected, userData.Data, expectedUser);
        ShouldBeNull(unexpected, userData.Data);

        // - Links
        userData.Links.Should().NotBeNull();
        userData.Links.Count.Should().Be(2);
        LinksForUserShouldBeCorrect(userData.Links, userId);

        // - Embedded
        userData.Embedded.Should().BeNull();
    }

    [Test]
    [Category("UnHappy")]
    public async Task When_A_User_Is_Requested_With_Invalid_Fields___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var userId = Guid.NewGuid();
        var url = $"/api/v1/users/{userId}?fields=fielddoesnotexist";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        problemDetails.Title.Should().Be("Validation error(s) occurred!");
        problemDetails.Detail.Should().Be("See the errors field for details.");
        problemDetails.Instance.Should().Be($"/api/v1/users/{userId}");
        problemDetails.Extensions.Should().NotBeNull();
        var traceId = problemDetails.Extensions.Where(_ => _.Key == "traceId").FirstOrDefault();
        traceId.Should().NotBeNull();
        problemDetails.Errors.Count().Should().Be(1);
        var error = problemDetails.Errors.First();
        error.Key.Should().Be("Fields");
        var value = error.Value.First();
        value.Should().Be("The fields value is invalid. e.g. fields=id,lastname");
    }

    private static void ShouldBeEquivalent(IList<string> fieldNames, Models.Response.v1.UserResponse actual, Domain.Models.User expected)
    {
        foreach (var fieldName in fieldNames)
        {
            ShouldBeEquivalent(fieldName, actual, expected);
        }
    }

    private static void ShouldBeEquivalent(string fieldName, Models.Response.v1.UserResponse actual, Domain.Models.User expected)
    {
        var actualValue = GetValue(actual, fieldName);
        var expectedValue = GetValue(expected, fieldName);
        actualValue.Should().Be(expectedValue);
    }

    private static void ShouldBeNull(IList<string> fieldNames, Models.Response.v1.UserResponse actual)
    {
        foreach (var fieldName in fieldNames)
        {
            ShouldBeNull(fieldName, actual);
        }
    }

    private static void ShouldBeNull(string fieldName, Models.Response.v1.UserResponse actual)
    {
        var actualValue = GetValue(actual, fieldName);
        actualValue.Should().BeNull();
    }
}
