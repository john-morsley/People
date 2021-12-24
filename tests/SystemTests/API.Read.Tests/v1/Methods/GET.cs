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
        var url = $"/api/v1/users?PageNumber=1&PageSize=10";
            
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
        var url = $"/api/v1/users?PageNumber=1&PageSize=10";

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
        var url = $"/api/v1/users?PageNumber=1&PageSize=10";

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
        var johnSmith = new Users.Domain.Models.User() { FirstName = "John", LastName = "Smith" };
        AddUserToDatabase(johnSmith);
        var johnMorsley = new Users.Domain.Models.User() { FirstName = "John", LastName = "Morsley" };
        AddUserToDatabase(johnMorsley);
        var joeBloggs = new Users.Domain.Models.User() { FirstName = "Joe", LastName = "Bloggs" };
        AddUserToDatabase(joeBloggs);
        var fredBloggs = new Users.Domain.Models.User() { FirstName = "Fred", LastName = "Bloggs" };
        AddUserToDatabase(fredBloggs);
        NumberOfUsersInDatabase().Should().Be(4);

        // Act...
        var url = $"/api/v1/users?orderBy=FirstName|Asc,LastName|Asc";
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
        firstUser.FirstName.Should().Be("Fred");
        firstUser.LastName.Should().Be("Bloggs");
        secondUser.FirstName.Should().Be("Joe");
        secondUser.LastName.Should().Be("Bloggs");
        thirdUser.FirstName.Should().Be("John");
        thirdUser.LastName.Should().Be("Morsley");
        fourthUser.FirstName.Should().Be("John");
        fourthUser.LastName.Should().Be("Smith");
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
        var url = $"/api/v1/users?orderBy=DateOfBirth|Asc";
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
    public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Invalid_Sorting___Then_TBC()
    {
        // Arrange...

        // Act...
        var url = $"/api/v1/users?orderBy=InvalidFieldName";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        //httpResponse.StatusCode.Should().Be(HttpStatusCode.OK); // What should this be? 422 maybe? Not a 500!
        //var response = await httpResponse.Content.ReadAsStringAsync();
        //response.Length.Should().BeGreaterThan(0);
    }
}
