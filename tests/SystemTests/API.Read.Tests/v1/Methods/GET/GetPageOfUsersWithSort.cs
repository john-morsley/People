namespace Users.API.Read.Tests.v1.Methods;

public class GetPageOfUsersWithSort : APIsTestBase<StartUp>
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

    private const string DateOfBirthAsc = "[Null]|[Null]|[Null]|[Null]|George Boole|Ada Lovelace|Tommy Flowers|Jane Goodall|John Cleese|Linus Torvalds";

    private const string DateOfBirthDesc = "Linus Torvalds|John Cleese|Jane Goodall|Tommy Flowers|Ada Lovelace|George Boole|[Null]|[Null]|[Null]|[Null]";

    [Test]
    [Category("Happy")]
    [TestCase("FirstName:asc,LastName:asc", TestUserData, FirstNameAscLastNameAsc)]
    [TestCase("LastName:asc,FirstName:asc", TestUserData, LastNameAscFirstNameAsc)]
    [TestCase("FirstName:desc,LastName:desc", TestUserData, FirstNameDescLastNameDesc)]
    [TestCase("LastName:desc,FirstName:desc", TestUserData, LastNameDescFirstNameDesc)]
    [TestCase("DateOfBirth:asc", TestUserData, DateOfBirthAsc)]
    [TestCase("DateOfBirth:desc", TestUserData, DateOfBirthDesc)]
    public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Valid_Sort___Then_200_OK_And_Users_Should_Be_Sorted(string validSort, string userData, string expectedOrder)
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var users = AddTestUsersToDatabase(userData);
        NumberOfUsersInDatabase().Should().Be(users.Count);

        // Act...
        var url = $"/api/v1/users?sort={validSort}";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(users.Count);

        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);

        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(users.Count);

        var index = 0;
        foreach(var name in expectedOrder.Split('|'))
        {
            var actualUser = pageOfUsers.Skip(index).Take(1).Single();
            index++;

            if (name == "[Null]") continue;

            var kvp = name.Split(' ');
            var firstName = kvp[0];
            var lastName = kvp[1];
            var expectedUser = users.Single(_ => _.FirstName == firstName && _.LastName == lastName);
            
            //actualUser.Id.Should().Be(expectedUser.Id);
            actualUser.FirstName.Should().Be(expectedUser.FirstName);
            actualUser.LastName.Should().Be(expectedUser.LastName);
            actualUser.Sex.Should().Be(expectedUser.Sex);
            actualUser.Gender.Should().Be(expectedUser.Gender);
            actualUser.DateOfBirth.Should().Be(expectedUser.DateOfBirth.InternationalFormat());
            
        }

        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);

        var pagination = JsonSerializer.Deserialize<Users.API.Models.Shared.Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be((uint)users.Count);
        pagination.PageSize.Should().Be(10);
    }

    [Test]
    [Category("Unhappy")]
    [TestCase("Id")]
    [TestCase("InvalidFieldName")]
    public async Task When_A_Page_Of_Users_Is_Requested_With_Invalid_Sorting___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues(string invalidSort)
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users?sort={invalidSort}";
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
        error.Key.Should().Be("Sort");
        var value = error.Value.First();
        value.Should().Be("The sort value is invalid. e.g. sort=lastname:asc,firstname:asc");
    }
}
