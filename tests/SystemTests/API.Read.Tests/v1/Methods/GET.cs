namespace Users.API.Read.Tests.v1.Methods;

public class GET : APIsTestBase<StartUp>
{
    [Test]
    [Category("Happy")]
    public async Task Given_User_Does_Not_Exist___When_That_User_Is_Requested___Then_204_NoContent()
    {
        // Arrange...
        var userId = Guid.NewGuid();

        var url = $"/api/v1/users/{userId}";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().Be(0);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_User_Exists___When_That_User_Is_Requested___Then_200_OK_And_User_Returned()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var expected = GenerateTestUser(userId);
        AddUserToDatabase(expected);

        var url = $"/api/v1/users/{userId}";
            
        // Act...
        var httpResponse = await _client.GetAsync(url);
            
        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var actual = DeserializeUserResponse(response);
        actual.Should().NotBeNull();
        actual.Id.Should().Be(expected.Id);
        actual.FirstName.Should().Be(expected.FirstName);
        actual.LastName.Should().Be(expected.LastName);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_No_Users_Exist___When_Page_Of_Users_Is_Requested___Then_204_NoContent()
    {
        // Arrange...
        var url = $"/api/v1/users?pageNumber=1&pageSize=10";
            
        // Act...
        var httpResponse = await _client.GetAsync(url);
            
        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().Be(0);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_One_User_Returned()
    {
        // Arrange...
        var user = GenerateTestUser();
        AddUserToDatabase(user);
        var url = $"/api/v1/users?pageNumber=1&PageSize=10";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);

        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(1);
        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);
        var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be(1);
        pagination.PageSize.Should().Be(10);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Multiple_Users_Exist___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_Users_Returned()
    {
        // Arrange...
        AddUsersToDatabase(15);
        NumberOfUsersInDatabase().Should().Be(15);
        var url = $"/api/v1/users?pageNumber=1&pageSize=10";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(10);
        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);
        var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().Be("http://localhost/api/v1/users?pageNumber=2&pageSize=10&orderBy=LastName");
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(2);
        pagination.TotalCount.Should().Be(15);
        pagination.PageSize.Should().Be(10);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Multiple_Users_Exist___When_A_Page_Of_Users_Is_Requested_Without_Parameters___Then_200_OK_And_Page_Of_Users_Returned_Using_Default_Values()
    {
        // Arrange...
        AddUsersToDatabase(5);
        var url = $"/api/v1/users";

        // Act...
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(5);
        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);
        var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be(5);
        pagination.PageSize.Should().Be(10);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Sorting_By_FirstName_And_LastName___Then_200_OK_And_Users_Returned_In_Correct_Order()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var johnDoe = new Users.Domain.Models.User() { FirstName = "John", LastName = "Doe" };
        AddUserToDatabase(johnDoe);
        var janeDoe = new Users.Domain.Models.User() { FirstName = "Jane", LastName = "Doe" };
        AddUserToDatabase(janeDoe);
        var fredBloggs = new Users.Domain.Models.User() { FirstName = "Fred", LastName = "Bloggs" };
        AddUserToDatabase(fredBloggs);
        var fayeBloggs = new Users.Domain.Models.User() { FirstName = "Faye", LastName = "Bloggs" };
        AddUserToDatabase(fayeBloggs);
        NumberOfUsersInDatabase().Should().Be(4);

        // Act...
        var url = $"/api/v1/users?orderBy=FirstName:Asc,LastName:Asc";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(4);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(4);
        var firstUser = pageOfUsers.Skip(0).Take(1).Single();
        var secondUser = pageOfUsers.Skip(1).Take(1).Single();
        var thirdUser = pageOfUsers.Skip(2).Take(1).Single();
        var fourthUser = pageOfUsers.Skip(3).Take(1).Single();
        firstUser.FirstName.Should().Be("Faye");
        firstUser.LastName.Should().Be("Bloggs");
        secondUser.FirstName.Should().Be("Fred");
        secondUser.LastName.Should().Be("Bloggs");
        thirdUser.FirstName.Should().Be("Jane");
        thirdUser.LastName.Should().Be("Doe");
        fourthUser.FirstName.Should().Be("John");
        fourthUser.LastName.Should().Be("Doe");
        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);
        var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be(4);
        pagination.PageSize.Should().Be(10);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Sorting_By_DateOfBirth___Then_200_OK_And_Users_Returned_In_Correct_Order()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var user1 = GenerateTestUser(1950, 4, 1);
        AddUserToDatabase(user1);
        var user2 = GenerateTestUser(1967, 11, 11);
        AddUserToDatabase(user2);
        var user3 = GenerateTestUser(1967, 11, 10);
        AddUserToDatabase(user3);
        var user4 = GenerateTestUser(1967, 10, 11);
        AddUserToDatabase(user4);
        var user5 = GenerateTestUser(1964, 2,29);
        AddUserToDatabase(user5);
        NumberOfUsersInDatabase().Should().Be(5);

        // Act...
        var url = $"/api/v1/users?orderBy=DateOfBirth:Asc";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(5);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(5);
        var firstUser = pageOfUsers.Skip(0).Take(1).Single();
        var secondUser = pageOfUsers.Skip(1).Take(1).Single();
        var thirdUser = pageOfUsers.Skip(2).Take(1).Single();
        var fourthUser = pageOfUsers.Skip(3).Take(1).Single();
        var fithUser = pageOfUsers.Skip(4).Take(1).Single();
        firstUser.DateOfBirth.Should().BeEquivalentTo(user1.DateOfBirth.InternationalFormat());
        secondUser.DateOfBirth.Should().BeEquivalentTo(user5.DateOfBirth.InternationalFormat());
        thirdUser.DateOfBirth.Should().BeEquivalentTo(user4.DateOfBirth.InternationalFormat());
        fourthUser.DateOfBirth.Should().BeEquivalentTo(user3.DateOfBirth.InternationalFormat());
        fithUser.DateOfBirth.Should().BeEquivalentTo(user2.DateOfBirth.InternationalFormat());
        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);
        var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be(5);
        pagination.PageSize.Should().Be(10);
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_A_Page_Of_Users_Is_Requested_With_Invalid_Sorting___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...

        // Act...
        var url = $"/api/v1/users?orderBy=InvalidFieldName";
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
        error.Key.Should().Be("OrderBy");
        var value = error.Value.First();
        value.Should().Be("The sort order is invalid.");
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Filtering_By_Sex___Then_200_OK_And_Users_Should_Be_Filtered()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var john = new Users.Domain.Models.User() { FirstName = "John", LastName = "Doe", Sex = Domain.Enumerations.Sex.Male };
        AddUserToDatabase(john);
        var jane = new Users.Domain.Models.User() { FirstName = "Jane", LastName = "Doe", Sex = Domain.Enumerations.Sex.Female };
        AddUserToDatabase(jane);
        var fred = new Users.Domain.Models.User() { FirstName = "Fred", LastName = "Bloggs", Sex = Domain.Enumerations.Sex.Male };
        AddUserToDatabase(fred);
        var faye = new Users.Domain.Models.User() { FirstName = "Faye", LastName = "Bloggs", Sex = Domain.Enumerations.Sex.Female };
        AddUserToDatabase(faye);
        NumberOfUsersInDatabase().Should().Be(4);

        // Act...
        var url = $"/api/v1/users?filter=sex:female";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(4);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(2);
        var firstUser = pageOfUsers.Skip(0).Take(1).Single();
        var secondUser = pageOfUsers.Skip(1).Take(1).Single();
        firstUser.FirstName.Should().Be(faye.FirstName);
        firstUser.LastName.Should().Be(faye.LastName);
        secondUser.FirstName.Should().Be(jane.FirstName);
        secondUser.LastName.Should().Be(jane.LastName);
        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);
        var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be(2);
        pagination.PageSize.Should().Be(10);
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_A_Page_Of_Users_Is_Requested_With_Invalid_Filter___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...

        // Act...
        var url = $"/api/v1/users?filter=InvalidFieldName:Invalid";
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
        value.Should().Be("The filter is invalid.");
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Search_Criteria___Then_200_OK_And_Users_Should_Be_Limited()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var johnDoe = new Users.Domain.Models.User() { FirstName = "John", LastName = "Doe" };
        AddUserToDatabase(johnDoe);
        var janeDoe = new Users.Domain.Models.User() { FirstName = "Jane", LastName = "Doe" };
        AddUserToDatabase(janeDoe);
        var saadMaan = new Users.Domain.Models.User() { FirstName = "Saad", LastName = "Man" };
        AddUserToDatabase(saadMaan);
        var whetFaartz = new Users.Domain.Models.User() { FirstName = "Whet", LastName = "Faartz" };
        AddUserToDatabase(whetFaartz);
        var fredBloggs = new Users.Domain.Models.User() { FirstName = "Fred", LastName = "Bloggs" };
        AddUserToDatabase(fredBloggs);
        var fayeBloggs = new Users.Domain.Models.User() { FirstName = "Faye", LastName = "Bloggs" };
        AddUserToDatabase(fayeBloggs);
        NumberOfUsersInDatabase().Should().Be(6);

        // Act...
        var url = $"/api/v1/users?search=aa";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(6);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);
        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(2);
        var firstUser = pageOfUsers.Skip(0).Take(1).Single();
        var secondUser = pageOfUsers.Skip(1).Take(1).Single();
        firstUser.FirstName.Should().Be(whetFaartz.FirstName);
        firstUser.LastName.Should().Be(whetFaartz.LastName);
        secondUser.FirstName.Should().Be(saadMaan.FirstName);
        secondUser.LastName.Should().Be(saadMaan.LastName);
        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);
        var pagination = JsonSerializer.Deserialize<Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be(2);
        pagination.PageSize.Should().Be(10);
    }
}
