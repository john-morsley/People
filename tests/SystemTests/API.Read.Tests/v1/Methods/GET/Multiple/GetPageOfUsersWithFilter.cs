namespace Users.API.Read.Tests.v1.Methods.GET.Multiple;

public class GetPageOfUsersWithFilter : APIsTestBase<StartUp>
{
    private const string UserDataForFilter = "Mark Pink,Sex:Male|" +
                                             "John Green,Sex:Male|" +
                                             "Jane Doe,Sex:Female|" +
                                             "John Doe,Sex:Male|" +
                                             "Tom Yellow,Sex:Male|" +
                                             "Jane Brown,Sex:Female";

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

    [Test]
    [Category("Happy")]
    [TestCase("LastName:Doe", UserDataForFilter, "Jane Doe|John Doe")]
    [TestCase("Sex:Male", UserDataForFilterWithAllSexes, "John Doe")]
    [TestCase("Sex:", UserDataForFilterWithAllSexes, "Mary Green|Dave Pink")]
    [TestCase("Gender:Cisgender", UserDataForFilterWithAllGenders, "Lisa Yellow")]
    [TestCase("Gender:", UserDataForFilterWithAllGenders, "Jane Doe|John Doe")]
    public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Valid_Filter___Then_200_OK_And_Users_Should_Be_Filtered(string validFilter, string userData, string expectedUsers)
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var users = AddTestUsersToDatabase(userData);
        NumberOfUsersInDatabase().Should().Be(users.Count);

        // Act...
        var url = $"/api/v1/users?filter={validFilter}";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(users.Count);

        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);

        var pageOfUsers = DeserializeEmbeddedUsers(response);
        pageOfUsers.Should().NotBeNull();

        var index = 0;
        foreach(var name in expectedUsers.Split('|'))
        {
            var kvp = name.Split(' ');
            var firstName = kvp[0];
            var lastName = kvp[1];
            var expectedUser = users.Single(_ => _.FirstName == firstName && _.LastName == lastName);
            var actualUser = pageOfUsers.Skip(index).Take(1).Single();
            //actualUser.Id.Should().Be(expectedUser.Id);
            actualUser.FirstName.Should().Be(expectedUser.FirstName);
            actualUser.LastName.Should().Be(expectedUser.LastName);
            actualUser.Sex.Should().Be(expectedUser.Sex);
            actualUser.Gender.Should().Be(expectedUser.Gender);
            actualUser.DateOfBirth.Should().Be(expectedUser.DateOfBirth.InternationalFormat());
            index++;
        }

        pageOfUsers.Count().Should().Be(index);

        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);

        var pagination = JsonSerializer.Deserialize<Users.API.Models.Shared.Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be((uint)index);
        pagination.PageSize.Should().Be(10);
    }
    
    [Test]
    [Category("Unhappy")]
    [TestCase("Id:ShouldNotBeUsed")]
    [TestCase("InvalidFieldName")]
    public async Task When_A_Page_Of_Users_Is_Requested_With_Invalid_Filter___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues(string invalidFilter)
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users?filter={invalidFilter}";
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
