namespace Users.API.Read.Tests.v1.Methods;

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

        var pageOfUsers = DeserializeListOfUserResponses(response);
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

    //[Test]
    //[Category("Happy")]
    //public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Filtering_By_Sex___Then_200_OK_And_Users_Should_Be_Filtered()
    //{
    //    // Arrange...
    //    NumberOfUsersInDatabase().Should().Be(0);
    //    var john = new Users.Domain.Models.User() { FirstName = "John", LastName = "Doe", Sex = Domain.Enumerations.Sex.Male };
    //    AddUserToDatabase(john);
    //    var jane = new Users.Domain.Models.User() { FirstName = "Jane", LastName = "Doe", Sex = Domain.Enumerations.Sex.Female };
    //    AddUserToDatabase(jane);
    //    var fred = new Users.Domain.Models.User() { FirstName = "Fred", LastName = "Bloggs", Sex = Domain.Enumerations.Sex.Male };
    //    AddUserToDatabase(fred);
    //    var faye = new Users.Domain.Models.User() { FirstName = "Faye", LastName = "Bloggs", Sex = Domain.Enumerations.Sex.Female };
    //    AddUserToDatabase(faye);
    //    NumberOfUsersInDatabase().Should().Be(4);

    //    // Act...
    //    var url = $"/api/v1/users?filter=sex:female";
    //    var httpResponse = await _client.GetAsync(url);

    //    // Assert...
    //    NumberOfUsersInDatabase().Should().Be(4);

    //    httpResponse.IsSuccessStatusCode.Should().BeTrue();
    //    httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var response = await httpResponse.Content.ReadAsStringAsync();
    //    response.Length.Should().BeGreaterThan(0);

    //    var pageOfUsers = DeserializeListOfUserResponses(response);
    //    pageOfUsers.Should().NotBeNull();
    //    pageOfUsers.Count().Should().Be(2);

    //    var firstUser = pageOfUsers.Skip(0).Take(1).Single();
    //    var secondUser = pageOfUsers.Skip(1).Take(1).Single();

    //    firstUser.FirstName.Should().Be(faye.FirstName);
    //    firstUser.LastName.Should().Be(faye.LastName);
    //    secondUser.FirstName.Should().Be(jane.FirstName);
    //    secondUser.LastName.Should().Be(jane.LastName);

    //    IEnumerable<string> values;
    //    httpResponse.Headers.TryGetValues("X-Pagination", out values);
    //    values.Should().NotBeNull();
    //    values.Count().Should().Be(1);

    //    var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
    //    pagination.PreviousPageLink.Should().BeNull();
    //    pagination.NextPageLink.Should().BeNull();
    //    pagination.CurrentPage.Should().Be(1);
    //    pagination.TotalPages.Should().Be(1);
    //    pagination.TotalCount.Should().Be(2);
    //    pagination.PageSize.Should().Be(10);
    //}

    //[Test]
    //[Category("Happy")]
    //public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Filtering_By_Empty_Sex___Then_200_OK_And_Users_Should_Be_Filtered()
    //{
    //    // Arrange...
    //    NumberOfUsersInDatabase().Should().Be(0);
    //    var john = new Users.Domain.Models.User() { FirstName = "John", LastName = "Doe", Sex = Domain.Enumerations.Sex.Male };
    //    AddUserToDatabase(john);
    //    var jane = new Users.Domain.Models.User() { FirstName = "Jane", LastName = "Doe" };
    //    AddUserToDatabase(jane);
    //    var fred = new Users.Domain.Models.User() { FirstName = "Fred", LastName = "Bloggs"  };
    //    AddUserToDatabase(fred);
    //    var faye = new Users.Domain.Models.User() { FirstName = "Faye", LastName = "Bloggs", Sex = Domain.Enumerations.Sex.Female };
    //    AddUserToDatabase(faye);
    //    NumberOfUsersInDatabase().Should().Be(4);

    //    // Act...
    //    var url = $"/api/v1/users?filter=sex:";
    //    var httpResponse = await _client.GetAsync(url);

    //    // Assert...
    //    NumberOfUsersInDatabase().Should().Be(4);

    //    httpResponse.IsSuccessStatusCode.Should().BeTrue();
    //    httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var response = await httpResponse.Content.ReadAsStringAsync();
    //    response.Length.Should().BeGreaterThan(0);

    //    var pageOfUsers = DeserializeListOfUserResponses(response);
    //    pageOfUsers.Should().NotBeNull();
    //    pageOfUsers.Count().Should().Be(2);

    //    var firstUser = pageOfUsers.Skip(0).Take(1).Single();
    //    var secondUser = pageOfUsers.Skip(1).Take(1).Single();

    //    firstUser.FirstName.Should().Be(fred.FirstName);
    //    firstUser.LastName.Should().Be(fred.LastName);
    //    secondUser.FirstName.Should().Be(jane.FirstName);
    //    secondUser.LastName.Should().Be(jane.LastName);

    //    IEnumerable<string> values;
    //    httpResponse.Headers.TryGetValues("X-Pagination", out values);
    //    values.Should().NotBeNull();
    //    values.Count().Should().Be(1);

    //    var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
    //    pagination.PreviousPageLink.Should().BeNull();
    //    pagination.NextPageLink.Should().BeNull();
    //    pagination.CurrentPage.Should().Be(1);
    //    pagination.TotalPages.Should().Be(1);
    //    pagination.TotalCount.Should().Be(2);
    //    pagination.PageSize.Should().Be(10);
    //}

    //[Test]
    //[Category("Happy")]
    //public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Filtering_By_Gender___Then_200_OK_And_Users_Should_Be_Filtered()
    //{
    //    // Arrange...
    //    NumberOfUsersInDatabase().Should().Be(0);
    //    var john = new Users.Domain.Models.User() { FirstName = "John", LastName = "Doe", Gender = Domain.Enumerations.Gender.Cisgender };
    //    AddUserToDatabase(john);
    //    var jane = new Users.Domain.Models.User() { FirstName = "Jane", LastName = "Doe", Gender = Domain.Enumerations.Gender.NonBinary };
    //    AddUserToDatabase(jane);
    //    var fred = new Users.Domain.Models.User() { FirstName = "Fred", LastName = "Bloggs", Gender = Domain.Enumerations.Gender.PreferNotToSay };
    //    AddUserToDatabase(fred);
    //    var faye = new Users.Domain.Models.User() { FirstName = "Faye", LastName = "Bloggs", Gender = Domain.Enumerations.Gender.Cisgender };
    //    AddUserToDatabase(faye);
    //    NumberOfUsersInDatabase().Should().Be(4);

    //    // Act...
    //    var url = $"/api/v1/users?filter=gender:cisgender";
    //    var httpResponse = await _client.GetAsync(url);

    //    // Assert...
    //    NumberOfUsersInDatabase().Should().Be(4);

    //    httpResponse.IsSuccessStatusCode.Should().BeTrue();
    //    httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var response = await httpResponse.Content.ReadAsStringAsync();
    //    response.Length.Should().BeGreaterThan(0);

    //    var pageOfUsers = DeserializeListOfUserResponses(response);
    //    pageOfUsers.Should().NotBeNull();
    //    pageOfUsers.Count().Should().Be(2);

    //    var firstUser = pageOfUsers.Skip(0).Take(1).Single();
    //    var secondUser = pageOfUsers.Skip(1).Take(1).Single();

    //    firstUser.FirstName.Should().Be(faye.FirstName);
    //    firstUser.LastName.Should().Be(faye.LastName);
    //    secondUser.FirstName.Should().Be(john.FirstName);
    //    secondUser.LastName.Should().Be(john.LastName);

    //    IEnumerable<string> values;
    //    httpResponse.Headers.TryGetValues("X-Pagination", out values);
    //    values.Should().NotBeNull();
    //    values.Count().Should().Be(1);

    //    var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
    //    pagination.PreviousPageLink.Should().BeNull();
    //    pagination.NextPageLink.Should().BeNull();
    //    pagination.CurrentPage.Should().Be(1);
    //    pagination.TotalPages.Should().Be(1);
    //    pagination.TotalCount.Should().Be(2);
    //    pagination.PageSize.Should().Be(10);
    //}

    //[Test]
    //[Category("Happy")]
    //public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Filtering_By_Empty_Gender___Then_200_OK_And_Users_Should_Be_Filtered()
    //{
    //    // Arrange...
    //    NumberOfUsersInDatabase().Should().Be(0);
    //    var john = new Users.Domain.Models.User() { FirstName = "John", LastName = "Doe", Gender = Domain.Enumerations.Gender.Cisgender };
    //    AddUserToDatabase(john);
    //    var jane = new Users.Domain.Models.User() { FirstName = "Jane", LastName = "Doe" };
    //    AddUserToDatabase(jane);
    //    var fred = new Users.Domain.Models.User() { FirstName = "Fred", LastName = "Bloggs", Gender = Domain.Enumerations.Gender.PreferNotToSay };
    //    AddUserToDatabase(fred);
    //    var faye = new Users.Domain.Models.User() { FirstName = "Faye", LastName = "Bloggs" };
    //    AddUserToDatabase(faye);
    //    NumberOfUsersInDatabase().Should().Be(4);

    //    // Act...
    //    var url = $"/api/v1/users?filter=gender:";
    //    var httpResponse = await _client.GetAsync(url);

    //    // Assert...
    //    NumberOfUsersInDatabase().Should().Be(4);

    //    httpResponse.IsSuccessStatusCode.Should().BeTrue();
    //    httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var response = await httpResponse.Content.ReadAsStringAsync();
    //    response.Length.Should().BeGreaterThan(0);

    //    var pageOfUsers = DeserializeListOfUserResponses(response);
    //    pageOfUsers.Should().NotBeNull();
    //    pageOfUsers.Count().Should().Be(2);

    //    var firstUser = pageOfUsers.Skip(0).Take(1).Single();
    //    var secondUser = pageOfUsers.Skip(1).Take(1).Single();

    //    firstUser.FirstName.Should().Be(faye.FirstName);
    //    firstUser.LastName.Should().Be(faye.LastName);
    //    secondUser.FirstName.Should().Be(jane.FirstName);
    //    secondUser.LastName.Should().Be(jane.LastName);

    //    IEnumerable<string> values;
    //    httpResponse.Headers.TryGetValues("X-Pagination", out values);
    //    values.Should().NotBeNull();
    //    values.Count().Should().Be(1);

    //    var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
    //    pagination.PreviousPageLink.Should().BeNull();
    //    pagination.NextPageLink.Should().BeNull();
    //    pagination.CurrentPage.Should().Be(1);
    //    pagination.TotalPages.Should().Be(1);
    //    pagination.TotalCount.Should().Be(2);
    //    pagination.PageSize.Should().Be(10);
    //}

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
