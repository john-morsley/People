namespace Morsley.UK.People.API.Read.System.Tests.Methods.GET.People;

public class GetPeopleTests : ReadApplicationTestFixture<ReadProgram>
{
    [Test]
    [Category("Happy")]
    public async Task Given_No_People_Exist___When_Page_Of_People_Is_Requested___Then_204_NoContent()
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        ReadDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/people?pageNumber={pageNumber}&PageSize={pageSize}";
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        ReadDatabase.NumberOfPeople().Should().Be(0);

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
    public async Task Given_One_Person_Exists___When_A_Page_Of_People_Is_Requested___Then_200_OK_And_Page_Returned(
        int numberOfPeople,
        int pageNumber,
        int pageSize,
        int expectedNumberOfPeople,
        bool hasBeforeLink,
        bool hasAfterLink)
    {
        // Arrange...
        ReadDatabase.NumberOfPeople().Should().Be(0);
        var expectedPeople = ReadDatabase.AddPeople(numberOfPeople);
        ReadDatabase.NumberOfPeople().Should().Be(numberOfPeople);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/people?pageNumber={pageNumber}&pageSize={pageSize}";
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        ReadDatabase.NumberOfPeople().Should().Be(numberOfPeople);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var personResource = DeserializePersonResource(content);
        personResource.Should().NotBeNull();

        // - User
        personResource.Data.Should().BeNull();

        // - Embedded
        personResource.Embedded.Should().NotBeNull();
        personResource.Embedded.Count.Should().Be(expectedNumberOfPeople);
        var embedded = personResource.Embedded;
        ShouldBeEquivalentTo(embedded, expectedPeople);

        // - Links
        var expectedNumberOfLinks = 1;
        if (hasBeforeLink) expectedNumberOfLinks++;
        if (hasAfterLink) expectedNumberOfLinks++;
        personResource.Links.Should().NotBeNull();
        personResource.Links.Count.Should().Be(expectedNumberOfLinks);
        LinksForPeopleShouldBeCorrect(personResource.Links, pageNumber, pageSize, expectedNumberOfPeople);
    }

    [Test]
    [Category("Happy")]
    [TestCase(0, 1, 1)]
    [TestCase(1, 2, 1)]
    public async Task Given_One_Person_Exists___When_People_Are_Requested___Then_204_NoContent(
        int numberOfPeople,
        int pageNumber,
        int pageSize)
    {
        // Arrange...
        ReadDatabase.NumberOfPeople().Should().Be(0);
        ReadDatabase.AddPeople(numberOfPeople);
        ReadDatabase.NumberOfPeople().Should().Be(numberOfPeople);

        var url = $"/api/people?pageNumber={pageNumber}&pageSize={pageSize}";

        await AuthenticateAsync(Username, Password);

        // Act...
        
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        ReadDatabase.NumberOfPeople().Should().Be(numberOfPeople);

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
    public async Task When_A_Page_Of_People_Is_Requested_With_Invalid_Page_Parameters___Then_400_BadRequest(
        int pageNumber,
        int pageSize,
        string errorKey,
        string errorValue)
    {
        // Arrange...
        ReadDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/people?pageNumber={pageNumber}&PageSize={pageSize}";
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        ReadDatabase.NumberOfPeople().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        //result.IsSuccessStatusCode.Should().BeTrue();
        //result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var validationErrors = new Dictionary<string, string> {{ errorKey, errorValue }};
        ProblemDetailsShouldContainValidationIssues(content, validationErrors);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Multiple_People_Exist___When_A_Page_Of_People_Is_Requested___Then_200_OK_And_Page_Of_People_Returned()
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        ReadDatabase!.NumberOfPeople().Should().Be(0);
        var expectedPeople = ReadDatabase.AddPeople(15);
        ReadDatabase!.NumberOfPeople().Should().Be(15);

        var url = $"/api/people?pageNumber={pageNumber}&PageSize={pageSize}";

        await AuthenticateAsync(Username, Password);

        // Act...
        var result = await HttpClient!.GetAsync(url);

        // Assert...
        ReadDatabase!.NumberOfPeople().Should().Be(15);

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
        ShouldBeEquivalentTo(personResource.Embedded, expectedPeople);

        // - Links
        personResource.Links.Should().NotBeNull();
        personResource.Links!.Count.Should().Be(2);
        LinksForPeopleShouldBeCorrect(personResource.Links, pageNumber, pageSize, 15);
    }
}