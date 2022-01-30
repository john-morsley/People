namespace Users.API.Read.Tests.v1.Methods.GET.Multiple;

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
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
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
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var pageOfUsers = DeserializeEmbeddedUsers(content);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(1);

        IEnumerable<string> values;
        result.Headers.TryGetValues("X-Pagination", out values);
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
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(15);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();

        // Users...
        content.Length.Should().BeGreaterThan(0);
        var pageOfUsers = DeserializeEmbeddedUsers(content);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(10);


        IEnumerable<string> values;
        result.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);

        // Pagination...
        var pagination = JsonSerializer.Deserialize<Users.API.Models.Shared.Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        HttpUtility.UrlDecode(pagination.NextPageLink).Should().Be("http://localhost/api/v1/users?pageNumber=2&pageSize=10&sort=LastName:asc,FirstName:asc");
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(2);
        pagination.TotalCount.Should().Be(15);
        pagination.PageSize.Should().Be(10);

        // ToDo -->
        //Add Links for pagination and links for each user!

        // Metadata Links...
        var hateoas = DeserializeMetadata(content);
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
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(5);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
        var pageOfUsers = DeserializeEmbeddedUsers(content);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(5);

        IEnumerable<string> values;
        result.Headers.TryGetValues("X-Pagination", out values);
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
}
