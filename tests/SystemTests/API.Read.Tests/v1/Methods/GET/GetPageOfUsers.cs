namespace Users.API.Read.Tests.v1.Methods;

public class GetPageOfUsers : APIsTestBase<StartUp>
{
    [Test]
    [Category("Happy")]
    public async Task Given_No_Users_Exist___When_Page_Of_Users_Is_Requested___Then_204_NoContent()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users?pageNumber=1&pageSize=10";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);

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
        NumberOfUsersInDatabase().Should().Be(0);
        var user = GenerateTestUser();
        AddUserToDatabase(user);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        var url = $"/api/v1/users?pageNumber=1&PageSize=10";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

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

        var pagination = JsonSerializer.Deserialize<Users.API.Models.Shared.Pagination>(values.FirstOrDefault());
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
        NumberOfUsersInDatabase().Should().Be(0);
        AddUsersToDatabase(15);
        NumberOfUsersInDatabase().Should().Be(15);

        // Act...
        var url = $"/api/v1/users?pageNumber=1&pageSize=10";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(15);

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

        var pagination = JsonSerializer.Deserialize<Users.API.Models.Shared.Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        HttpUtility.UrlDecode(pagination.NextPageLink).Should().Be("http://localhost/api/v1/users?pageNumber=2&pageSize=10&sort=LastName:asc,FirstName:asc");
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
        NumberOfUsersInDatabase().Should().Be(0);
        AddUsersToDatabase(5);
        NumberOfUsersInDatabase().Should().Be(5);

        // Act...
        var url = $"/api/v1/users";
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

        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);

        var pagination = JsonSerializer.Deserialize<Users.API.Models.Shared.Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be(5);
        pagination.PageSize.Should().Be(10);
    }

    [Test]
    [Category("UnHappy")]
    public async Task When_A_User_Is_Requested_With_Invalid_Fields___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var userId = Guid.NewGuid();
        var url = $"/api/v1/users/{userId}?fields=fielddoesnotexist";
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
}
