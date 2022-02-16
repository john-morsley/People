namespace Users.API.Read.Tests.v1.Methods.GET.Multiple;

public class GetPageOfUsersWithFilter : APIsTestBase<StartUp>
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
    public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Valid_Filter___Then_200_OK_And_Users_Should_Be_Filtered(
        string validFilter, 
        string testUsersData, 
        string expectedUsers)
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        NumberOfUsersInDatabase().Should().Be(0);
        var users = AddTestUsersToDatabase(testUsersData);
        NumberOfUsersInDatabase().Should().Be(users.Count);

        var url = $"/api/v1/users?filter={validFilter}";

        // Act...
        var response = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(users.Count);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userData = DeserializeUserData(content);
        userData.Should().NotBeNull();

        // - User
        userData.User.Should().BeNull();

        // - Embedded
        userData.Embedded.Should().NotBeNull();
        userData.Embedded.Count.Should().Be(expectedUsers.Split('|').Length);
        foreach(var name in expectedUsers.Split('|'))
        {
            var kvp = name.Split(' ');
            var firstName = kvp[0];
            var lastName = kvp[1];
            var expectedUser = users.Single(_ => _.FirstName == firstName && _.LastName == lastName);
            var actualUserData = userData.Embedded.Single(_ => _.User.FirstName == firstName && _.User.LastName == lastName);
            actualUserData.Should().NotBeNull();

            // User...
            actualUserData.User.Id.Should().Be(expectedUser.Id);
            actualUserData.User.Sex.Should().Be(expectedUser.Sex);
            actualUserData.User.Gender.Should().Be(expectedUser.Gender);

            // Links...
            actualUserData.Links.Should().NotBeNull();
            actualUserData.Links.Count.Should().Be(2);
            LinksForUserShouldBeCorrect(actualUserData.Links, actualUserData.User.Id);

            // Embedded...
            actualUserData.Embedded.Should().BeNull();
            
            //actualUser.DateOfBirth.Should().Be(expectedUser.DateOfBirth.InternationalFormat());
        }

        // - Links
        userData.Links.Should().NotBeNull();
        userData.Links.Count.Should().Be(1);
        LinksForPageOfUsersShouldBeCorrect(userData.Links, pageNumber, pageSize, filter: validFilter, totalNumber: users.Count);
    }

    [Test]
    [Category("Unhappy")]
    [TestCase("Id:ShouldNotBeUsed")]
    [TestCase("InvalidFieldName")]
    public async Task When_A_Page_Of_Users_Is_Requested_With_Invalid_Filter___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues(string invalidFilter)
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var url = $"/api/v1/users?filter={invalidFilter}";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);

        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);

        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(response);
        problemDetails.Should().NotBeNull();
        problemDetails.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        problemDetails.Title.Should().Be("Validation error(s) occurred!");
        problemDetails.Detail.Should().Be("See the errors field for details.");
        problemDetails.Instance.Should().Be("/api/v1/users");
        problemDetails.Extensions.Should().NotBeNull();
        var traceId = problemDetails.Extensions.Where(_ => _.Key == "traceId").FirstOrDefault();
        traceId.Should().NotBeNull();
        problemDetails.Errors.Count().Should().Be(1);
        var error = problemDetails.Errors.First();
        error.Key.Should().Be("Filter");
        var value = error.Value.First();
        value.Should().Be("The filter value is invalid. e.g. filter=sex:male");
    }
}
