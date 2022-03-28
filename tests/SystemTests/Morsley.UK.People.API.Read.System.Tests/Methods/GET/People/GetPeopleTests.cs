namespace Morsley.UK.People.API.Read.System.Tests.Methods.GET.People;

public class GetPeopleTests : SecuredApplicationTestFixture<ReadProgram>
{
    [Test]
    [Category("Happy")]
    public async Task Given_No_Users_Exist___When_Page_Of_Users_Is_Requested___Then_204_NoContent()
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/people?pageNumber={pageNumber}&PageSize={pageSize}";
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

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
    public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_Users_Returned(
        int numberOfUsers,
        int pageNumber,
        int pageSize,
        int expectedNumberOfUsers,
        bool hasBeforeLink,
        bool hasAfterLink)
    {
        // Arrange...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);
        var expectedUsers = DatabaseTestFixture.AddPeopleToDatabase(numberOfUsers);
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(numberOfUsers);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/people?pageNumber={pageNumber}&pageSize={pageSize}";
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(numberOfUsers);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userResource = DeserializePersonResource(content);
        userResource.Should().NotBeNull();

        // - User
        userResource.Data.Should().BeNull();

        // - Embedded
        userResource.Embedded.Should().NotBeNull();
        userResource.Embedded.Count.Should().Be(expectedNumberOfUsers);
        var embedded = userResource.Embedded;
        ShouldBeEquivalentTo(embedded, expectedUsers);

        // - Links
        var expectedNumberOfLinks = 1;
        if (hasBeforeLink) expectedNumberOfLinks++;
        if (hasAfterLink) expectedNumberOfLinks++;
        userResource.Links.Should().NotBeNull();
        userResource.Links.Count.Should().Be(expectedNumberOfLinks);
        LinksForPeopleShouldBeCorrect(userResource.Links, pageNumber, pageSize, expectedNumberOfUsers);
    }

    [Test]
    [Category("Happy")]
    [TestCase(0, 1, 1)]
    [TestCase(1, 2, 1)]
    public async Task Given_One_Person_User_Exists___When_People_Are_Requested___Then_204_NoContent(
        int numberOfUsers,
        int pageNumber,
        int pageSize)
    {
        // Arrange...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);
        DatabaseTestFixture.AddPeopleToDatabase(numberOfUsers);
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(numberOfUsers);

        var url = $"/api/people?pageNumber={pageNumber}&pageSize={pageSize}";

        await AuthenticateAsync(Username, Password);

        // Act...
        
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(numberOfUsers);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);

        //var userData = DeserializePersonResource(content);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="errorKey"></param>
    /// <param name="errorValue"></param>
    /// <notes>
    /// ToDo --> Fix this, so it validates the input and returns a problem details object
    /// </notes>
    [Test]
    [Category("Unhappy")]
    [TestCase(0, 1, "PageNumber", "The pageNumber value is invalid. It must be greater than zero.")]
    [TestCase(1, 0, "PageSize", "The pageSize value is invalid. It must be greater than zero.")]
    public async Task When_A_Page_Of_Users_Is_Requested_With_Invalid_Page_Parameters___Then_400_BadRequest(
        int pageNumber,
        int pageSize,
        string errorKey,
        string errorValue)
    {
        // Arrange...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/people?pageNumber={pageNumber}&PageSize={pageSize}";
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        DatabaseTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        //result.IsSuccessStatusCode.Should().BeTrue();
        //result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var validationErrors = new Dictionary<string, string> {{ errorKey, errorValue }};
        ProblemDetailsShouldContainValidationIssues(content, validationErrors);
    }

    //[Test]
    //[Category("Happy")]
    //public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_Users_Returned()
    //{
    //    // Arrange...
    //    const int pageNumber = 1;
    //    const int pageSize = 10;

    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(0);
    //    var expectedUser = GeneratedTestPerson();
    //    AddPersonToDatabase(expectedUser);
    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(1);

    //    var url = $"/api/v1/users?pageNumber={pageNumber}&PageSize={pageSize}";

    //    // Act...
    //    var result = await HttpClient.GetAsync(url);

    //    // Assert...
    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(1);

    //    result.IsSuccessStatusCode.Should().BeTrue();
    //    result.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var content = await result.Content.ReadAsStringAsync();
    //    content.Length.Should().BeGreaterThan(0);

    //    var userData = DeserializePersonResource(content);
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

    [Test]
    [Category("Happy")]
    public async Task Given_Multiple_Users_Exist___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_Users_Returned()
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        DatabaseTestFixture!.NumberOfPeopleInDatabase().Should().Be(0);
        var expectedUsers = DatabaseTestFixture.AddPeopleToDatabase(15);
        DatabaseTestFixture!.NumberOfPeopleInDatabase().Should().Be(15);

        var url = $"/api/people?pageNumber={pageNumber}&PageSize={pageSize}";

        await AuthenticateAsync(Username, Password);

        // Act...
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        DatabaseTestFixture!.NumberOfPeopleInDatabase().Should().Be(15);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
        var personResource = DeserializePersonResource(content);
        personResource.Should().NotBeNull();

        // - User
        personResource!.Data.Should().BeNull();

        // - Embedded
        personResource.Embedded.Should().NotBeNull();
        personResource.Embedded!.Count.Should().Be(10);
        ShouldBeEquivalentTo(personResource.Embedded, expectedUsers);

        // - Links
        personResource.Links.Should().NotBeNull();
        personResource.Links!.Count.Should().Be(2);
        LinksForPeopleShouldBeCorrect(personResource.Links, pageNumber, pageSize, 15);
    }

    //[Test]
    //[Category("Happy")]
    //public async Task Given_Multiple_Users_Exist___When_A_Page_Of_Users_Is_Requested___Then_200_OK_And_Page_Of_Users_Returned_With_Co()
    //{
    //    // Arrange...
    //    const int pageNumber = 1;
    //    const int pageSize = 10;

    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(0);
    //    var expectedUsers = AddPeopleToDatabase(15);
    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(15);

    //    var url = $"/api/v1/users?pageNumber={pageNumber}&PageSize={pageSize}";

    //    // Act...
    //    var result = await HttpClient.GetAsync(url);

    //    // Assert...
    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(15);

    //    result.IsSuccessStatusCode.Should().BeTrue();
    //    result.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var content = await result.Content.ReadAsStringAsync();
    //    content.Length.Should().BeGreaterThan(0);
    //    var userData = DeserializePersonResource(content);
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
    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(0);
    //    var expectedUsers = AddPeopleToDatabase(5);
    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(5);

    //    const string url = "/api/v1/users";

    //    // Act...
    //    var result = await HttpClient.GetAsync(url);

    //    // Assert...
    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(5);

    //    result.IsSuccessStatusCode.Should().BeTrue();
    //    result.StatusCode.Should().Be(HttpStatusCode.OK);

    //    var content = await result.Content.ReadAsStringAsync();
    //    content.Length.Should().BeGreaterThan(0);

    //    var userData = DeserializePersonResource(content);
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