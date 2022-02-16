namespace Users.API.Read.Tests.v1.Methods.GET.Multiple;

public class GetPageOfUsers : APIsTestBase<StartUp>
{
    [Test]
    [Category("Happy")]
    public async Task Given_No_Users_Exist___When_Page_Of_Users_Is_Requested___Then_204_NoContent()
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        NumberOfUsersInDatabase().Should().Be(0);

        var url = $"/api/v1/users?pageNumber={pageNumber}&PageSize={pageSize}";

        // Act...
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
        const int pageNumber = 1;
        const int pageSize = 10;

        NumberOfUsersInDatabase().Should().Be(0);
        var expectedUser = GenerateTestUser();
        AddUserToDatabase(expectedUser);
        NumberOfUsersInDatabase().Should().Be(1);

        var url = $"/api/v1/users?pageNumber={pageNumber}&PageSize={pageSize}";

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

        // - User
        userData.User.Should().BeNull();

        // - Embedded
        userData.Embedded.Should().NotBeNull();
        userData.Embedded.Count.Should().Be(1);
        ShouldBeEquivalentTo(userData.Embedded, expectedUser);

        // - Links
        userData.Links.Should().NotBeNull();
        userData.Links.Count.Should().Be(1);
        LinksForPageOfUsersShouldBeCorrect(userData.Links, pageNumber, pageSize);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Multiple_Users_Exist___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_Users_Returned()
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        NumberOfUsersInDatabase().Should().Be(0);
        var expectedUsers = AddUsersToDatabase(15);
        NumberOfUsersInDatabase().Should().Be(15);

        var url = $"/api/v1/users?pageNumber={pageNumber}&PageSize={pageSize}";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(15);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
        var userData = DeserializeUserData(content);
        userData.Should().NotBeNull();

        // - User
        userData.User.Should().BeNull();

        // - Embedded
        userData.Embedded.Should().NotBeNull();
        userData.Embedded.Count.Should().Be(10);
        ShouldBeEquivalentTo(userData.Embedded, expectedUsers);

        // - Links
        userData.Links.Should().NotBeNull();
        userData.Links.Count.Should().Be(2);
        LinksForPageOfUsersShouldBeCorrect(userData.Links, pageNumber, pageSize);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested_Without_Parameters___Then_200_OK_And_Page_Of_One_User_Returned_Using_Default_Values()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var expectedUser = GenerateTestUser();
        AddUserToDatabase(expectedUser);
        NumberOfUsersInDatabase().Should().Be(1);

        const string url = "/api/v1/users";

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

        // - User
        userData.User.Should().BeNull();

        // - Embedded
        userData.Embedded.Should().NotBeNull();
        userData.Embedded.Count.Should().Be(1);
        ShouldBeEquivalentTo(userData.Embedded, expectedUser);

        // - Links
        userData.Links.Should().NotBeNull();
        userData.Links.Count.Should().Be(1);
        LinksForPageOfUsersShouldBeCorrect(userData.Links,  Users.API.Models.Constants.Defaults.DefaultPageNumber, Users.API.Models.Constants.Defaults.DefaultPageSize);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Multiple_Users_Exist___When_A_Page_Of_Users_Is_Requested_Without_Parameters___Then_200_OK_And_Page_Of_Users_Returned_Using_Default_Values()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var expectedUsers = AddUsersToDatabase(5);
        NumberOfUsersInDatabase().Should().Be(5);

        const string url = "/api/v1/users";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(5);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userData = DeserializeUserData(content);
        userData.Should().NotBeNull();

        // - User
        userData.User.Should().BeNull();

        // - Links
        userData.Links.Should().NotBeNull();
        userData.Links.Count.Should().Be(1);
        LinksForPageOfUsersShouldBeCorrect(userData.Links, Users.API.Models.Constants.Defaults.DefaultPageNumber, Users.API.Models.Constants.Defaults.DefaultPageSize);

        // - Embedded
        userData.Embedded.Should().NotBeNull();
        userData.Embedded.Count.Should().Be(5);
        ShouldBeEquivalentTo(userData.Embedded, expectedUsers);
    }
}