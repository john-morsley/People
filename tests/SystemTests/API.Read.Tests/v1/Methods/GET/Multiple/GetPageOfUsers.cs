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
    [TestCase(1, 1, 1, 1, false, false)]
    [TestCase(2, 1, 1, 1, false, true)]
    [TestCase(3, 2, 1, 1, true, true)]
    //[TestCase(30)]
    //[TestCase(100)]
    public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_Users_Returned(
        int numberOfUsers,
        int pageNumber,
        int pageSize,
        int expectedNumberOfUsers,
        bool hasBeforeLink,
        bool hasAfterLink)
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var expectedUsers = AddUsersToDatabase(numberOfUsers);
        NumberOfUsersInDatabase().Should().Be(numberOfUsers);

        var url = $"/api/v1/users?pageNumber={pageNumber}&PageSize={pageSize}";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(numberOfUsers);

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
        userData.Embedded.Count.Should().Be(expectedNumberOfUsers);
        ShouldBeEquivalentTo(userData.Embedded, expectedUsers);

        // - Links
        var expectedNumberOfLinks = 1;
        if (hasBeforeLink) expectedNumberOfLinks++;
        if (hasAfterLink) expectedNumberOfLinks++;
        userData.Links.Should().NotBeNull();
        userData.Links.Count.Should().Be(expectedNumberOfLinks);
        LinksForPageOfUsersShouldBeCorrect(userData.Links, pageNumber, pageSize, expectedNumberOfUsers);
    }

    [Test]
    [Category("Happy")]
    [TestCase(0, 1, 1)]
    [TestCase(1, 2, 1)]
    //[TestCase(30)]
    //[TestCase(100)]
    public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested___Then_204_NoContent(
        int numberOfUsers,
        int pageNumber,
        int pageSize)
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        //var expectedUser = GenerateTestUser();
        //AddUserToDatabase(expectedUser);
        AddUsersToDatabase(numberOfUsers);
        NumberOfUsersInDatabase().Should().Be(numberOfUsers);

        var url = $"/api/v1/users?pageNumber={pageNumber}&PageSize={pageSize}";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(numberOfUsers);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);

        //var userData = DeserializeUserData(content);
        //userData.Should().NotBeNull();

        // - User
        //userData.User.Should().BeNull();

        // - Embedded
        //userData.Embedded.Should().NotBeNull();
        //userData.Embedded.Count.Should().Be(expectedNumberOfUsers);
        //ShouldBeEquivalentTo(userData.Embedded, expectedUser);

        // - Links
        //userData.Links.Should().NotBeNull();
        //userData.Links.Count.Should().Be(1);
        //LinksForPageOfUsersShouldBeCorrect(userData.Links, pageNumber, pageSize);
    }

    [Test]
    [Category("Unhappy")]
    [TestCase(0, 0, 0)]
    [TestCase(0, 1, 0)]
    [TestCase(0, 0, 1)]
    public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested___Then_500_InternalServerError(
        int numberOfUsers,
        int pageNumber,
        int pageSize)
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        AddUsersToDatabase(numberOfUsers);
        NumberOfUsersInDatabase().Should().Be(numberOfUsers);

        var url = $"/api/v1/users?pageNumber={pageNumber}&PageSize={pageSize}";

        // Act...
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(numberOfUsers);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        // ToDo --> Process Error Object
    }

    //[Test]
    //[Category("Happy")]
    //public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_Users_Returned()
    //{
    //    // Arrange...
    //    const int pageNumber = 1;
    //    const int pageSize = 10;

    //    NumberOfUsersInDatabase().Should().Be(0);
    //    var expectedUser = GenerateTestUser();
    //    AddUserToDatabase(expectedUser);
    //    NumberOfUsersInDatabase().Should().Be(1);

    //    var url = $"/api/v1/users?pageNumber={pageNumber}&PageSize={pageSize}";

    //    // Act...
    //    var result = await _client.GetAsync(url);

    //    // Assert...
    //    NumberOfUsersInDatabase().Should().Be(1);

    //    result.IsSuccessStatusCode.Should().BeTrue();
    //    result.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var content = await result.Content.ReadAsStringAsync();
    //    content.Length.Should().BeGreaterThan(0);

    //    var userData = DeserializeUserData(content);
    //    userData.Should().NotBeNull();

    //    // - User
    //    userData.User.Should().BeNull();

    //    // - Embedded
    //    userData.Embedded.Should().NotBeNull();
    //    userData.Embedded.Count.Should().Be(1);
    //    ShouldBeEquivalentTo(userData.Embedded, expectedUser);

    //    // - Links
    //    userData.Links.Should().NotBeNull();
    //    userData.Links.Count.Should().Be(1);
    //    LinksForPageOfUsersShouldBeCorrect(userData.Links, pageNumber, pageSize);
    //}

    //[Test]
    //[Category("Happy")]
    //public async Task Given_Multiple_Users_Exist___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_Users_Returned()
    //{
    //    // Arrange...
    //    const int pageNumber = 1;
    //    const int pageSize = 10;

    //    NumberOfUsersInDatabase().Should().Be(0);
    //    var expectedUsers = AddUsersToDatabase(15);
    //    NumberOfUsersInDatabase().Should().Be(15);

    //    var url = $"/api/v1/users?pageNumber={pageNumber}&PageSize={pageSize}";

    //    // Act...
    //    var result = await _client.GetAsync(url);

    //    // Assert...
    //    NumberOfUsersInDatabase().Should().Be(15);

    //    result.IsSuccessStatusCode.Should().BeTrue();
    //    result.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var content = await result.Content.ReadAsStringAsync();
    //    content.Length.Should().BeGreaterThan(0);
    //    var userData = DeserializeUserData(content);
    //    userData.Should().NotBeNull();

    //    // - User
    //    userData.User.Should().BeNull();

    //    // - Embedded
    //    userData.Embedded.Should().NotBeNull();
    //    userData.Embedded.Count.Should().Be(10);
    //    ShouldBeEquivalentTo(userData.Embedded, expectedUsers);

    //    // - Links
    //    userData.Links.Should().NotBeNull();
    //    userData.Links.Count.Should().Be(2);
    //    LinksForPageOfUsersShouldBeCorrect(userData.Links, pageNumber, pageSize);
    //}

    //[Test]
    //[Category("Happy")]
    //public async Task Given_Multiple_Users_Exist___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_Users_Returned_With_Co()
    //{
    //    // Arrange...
    //    const int pageNumber = 1;
    //    const int pageSize = 10;

    //    NumberOfUsersInDatabase().Should().Be(0);
    //    var expectedUsers = AddUsersToDatabase(15);
    //    NumberOfUsersInDatabase().Should().Be(15);

    //    var url = $"/api/v1/users?pageNumber={pageNumber}&PageSize={pageSize}";

    //    // Act...
    //    var result = await _client.GetAsync(url);

    //    // Assert...
    //    NumberOfUsersInDatabase().Should().Be(15);

    //    result.IsSuccessStatusCode.Should().BeTrue();
    //    result.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var content = await result.Content.ReadAsStringAsync();
    //    content.Length.Should().BeGreaterThan(0);
    //    var userData = DeserializeUserData(content);
    //    userData.Should().NotBeNull();

    //    // - User
    //    userData.User.Should().BeNull();

    //    // - Embedded
    //    userData.Embedded.Should().NotBeNull();
    //    userData.Embedded.Count.Should().Be(10);
    //    ShouldBeEquivalentTo(userData.Embedded, expectedUsers);

    //    // - Links
    //    userData.Links.Should().NotBeNull();
    //    userData.Links.Count.Should().Be(2);
    //    LinksForPageOfUsersShouldBeCorrect(userData.Links, pageNumber, pageSize);
    //}

    //[Test]
    //[Category("Happy")]
    //public async Task Given_Multiple_Users_Exist___When_A_Page_Of_Users_Is_Requested_Without_Parameters___Then_200_OK_And_Page_Of_Users_Returned_Using_Default_Values()
    //{
    //    // Arrange...
    //    NumberOfUsersInDatabase().Should().Be(0);
    //    var expectedUsers = AddUsersToDatabase(5);
    //    NumberOfUsersInDatabase().Should().Be(5);

    //    const string url = "/api/v1/users";

    //    // Act...
    //    var result = await _client.GetAsync(url);

    //    // Assert...
    //    NumberOfUsersInDatabase().Should().Be(5);

    //    result.IsSuccessStatusCode.Should().BeTrue();
    //    result.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var content = await result.Content.ReadAsStringAsync();
    //    content.Length.Should().BeGreaterThan(0);

    //    var userData = DeserializeUserData(content);
    //    userData.Should().NotBeNull();

    //    // - User
    //    userData.User.Should().BeNull();

    //    // - Links
    //    userData.Links.Should().NotBeNull();
    //    userData.Links.Count.Should().Be(1);
    //    LinksForPageOfUsersShouldBeCorrect(userData.Links, Users.API.Models.Constants.Defaults.DefaultPageNumber, Users.API.Models.Constants.Defaults.DefaultPageSize);

    //    // - Embedded
    //    userData.Embedded.Should().NotBeNull();
    //    userData.Embedded.Count.Should().Be(5);
    //    ShouldBeEquivalentTo(userData.Embedded, expectedUsers);
    //}
}