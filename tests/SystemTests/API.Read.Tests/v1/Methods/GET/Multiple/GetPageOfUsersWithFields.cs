namespace Users.API.Read.Tests.v1.Methods.GET.Multiple;

public class GetPageOfUsersWithFields : APIsTestBase<StartUp>
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

        NumberOfUsersInDatabase().Should().Be(0);

        var user = GenerateTestUser();
        AddUserToDatabase(user);
        var users = new List<User> { user };
        NumberOfUsersInDatabase().Should().Be(1);

        var url = $"/api/v1/users?fields={validFields}";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userData = DeserializeUserData(content);
        userData.Should().NotBeNull();

        var validFieldsIncludingId = AddToFieldsIfMissing("Id", validFields);
        var (expected, unexpected) = DetermineExpectedAndUnexpectedFields(validFieldsIncludingId);

        // - User
        userData.User.Should().BeNull();

        // - Embedded
        userData.Embedded.Should().NotBeNull();
        userData.Embedded.Count.Should().Be(1);
        ShouldBeEquivalent(expected, userData.Embedded, users);
        ShouldBeNull(unexpected, userData.Embedded);
        LinksForUsersShouldBeCorrect(userData.Embedded);

        // - Links
        userData.Links.Should().NotBeNull();
        userData.Links.Count.Should().Be(1);
        LinksForPageOfUsersShouldBeCorrect(userData.Links, pageNumber, pageSize, fields: validFields, totalNumber: 1);
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_A_Page_Of_Users_Is_Requested_With_Invalid_Fields___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        const string url = "/api/v1/users?fields=fielddoesnotexist";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var response = await result.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);

        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(response);
        problemDetails.Should().NotBeNull();
        problemDetails.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        problemDetails.Title.Should().Be("Validation error(s) occurred!");
        problemDetails.Detail.Should().Be("See the errors field for details.");
        problemDetails.Instance.Should().Be($"/api/v1/users");
        problemDetails.Extensions.Should().NotBeNull();
        var traceId = problemDetails.Extensions.FirstOrDefault(_ => _.Key == "traceId");
        traceId.Should().NotBeNull();
        problemDetails.Errors.Count().Should().Be(1);
        var error = problemDetails.Errors.First();
        error.Key.Should().Be("Fields");
        var value = error.Value.First();
        value.Should().Be("The fields value is invalid. e.g. fields=id,lastname");
    }

    private static void ShouldBeEquivalent(
        IEnumerable<string> fieldNames, 
        IEnumerable<Users.API.Models.Shared.UserData> embedded,
        IEnumerable<Users.Domain.Models.User> expectedUsers)
    {
        foreach (var userData in embedded)
        {
            userData.Should().NotBeNull();
            userData.User.Should().NotBeNull();
            userData.Links.Should().NotBeNull();
            userData.Embedded.Should().BeNull();
            var expectedUser = expectedUsers.SingleOrDefault(_ => _.Id == userData.User.Id);
            expectedUser.Should().NotBeNull();
            ShouldBeEquivalent(fieldNames, userData.User, expectedUser);
        }
    }

    private static void ShouldBeEquivalent(
        IEnumerable<string> fieldNames,
        Models.Response.v1.UserResponse actual,
        Users.Domain.Models.User expected)
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

    private static void ShouldBeNull(
        IEnumerable<string> fieldNames,
        IEnumerable<Users.API.Models.Shared.UserData> embedded)
    {
        foreach (var userData in embedded)
        {
            userData.Should().NotBeNull();
            userData.User.Should().NotBeNull();
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