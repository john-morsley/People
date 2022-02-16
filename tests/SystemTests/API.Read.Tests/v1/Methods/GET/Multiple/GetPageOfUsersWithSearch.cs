namespace Users.API.Read.Tests.v1.Methods.GET.Multiple;

public class GetPageOfUsersWithSearch : APIsTestBase<StartUp>
{
    private const string UserDataForSearch = "Saad Man|" +
                                             "Whet Faartz|" +
                                             "Jane Doe|" +
                                             "John Doe|" +
                                             "Tom Kuntz|" +
                                             "Tom Adamski";

    [Test]
    [Category("Happy")]
    [TestCase("Doe", UserDataForSearch, "Jane Doe|John Doe")]
    [TestCase("Tom", UserDataForSearch, "Tom Kuntz|Tom Adamski")]
    [TestCase("aa", UserDataForSearch, "Saad Man|Whet Faartz")]
    [TestCase("tz", UserDataForSearch, "Whet Faartz|Tom Kuntz")]
    public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Search_Criteria___Then_200_OK_And_Users_Should_Be_Limited(string search, string usersData, string expectedUsers)
    {
        // Arrange...
        const int pageNumber = 1;
        const int pageSize = 10;

        NumberOfUsersInDatabase().Should().Be(0);
        var numberOfExpectedUsers = UserDataForSearch.Split('|').Length;
        var users = AddTestUsersToDatabase(usersData);
        NumberOfUsersInDatabase().Should().Be(numberOfExpectedUsers);

        // Act...
        var url = $"/api/v1/users?search={search}";
        var response = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(numberOfExpectedUsers);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userData = DeserializeUserData(content);
        userData.Should().NotBeNull();

        // User...
        userData.User.Should().BeNull();

        // Links...
        userData.Links.Should().NotBeNull();
        userData.Links.Count.Should().Be(1);
        LinksForPageOfUsersShouldBeCorrect(userData.Links, pageNumber, pageSize, search: search, totalNumber: numberOfExpectedUsers);

        // Embedded...
        userData.Embedded.Should().NotBeNull();
        userData.Embedded.Count.Should().Be(expectedUsers.Split('|').Length);
        foreach (var name in expectedUsers.Split('|'))
        {
            var kvp = name.Split(' ');
            var firstName = kvp[0];
            var lastName = kvp[1];
            var expectedUser = users.Single(_ => _.FirstName == firstName && _.LastName == lastName);
            var actualUserData = userData.Embedded.Single(_ => _.User.FirstName == firstName && _.User.LastName == lastName);
            actualUserData.Should().NotBeNull();

            // User...
            actualUserData.User.Id.Should().Be(expectedUser.Id);
            actualUserData.User.Sex.Should().Be(expectedUser.Sex);
            actualUserData.User.Gender.Should().Be(expectedUser.Gender);

            // Links...
            actualUserData.Links.Should().NotBeNull();
            actualUserData.Links.Count.Should().Be(2);
            LinksForUserShouldBeCorrect(actualUserData.Links, actualUserData.User.Id);

            // Embedded...
            actualUserData.Embedded.Should().BeNull();

            //actualUser.DateOfBirth.Should().Be(expectedUser.DateOfBirth.InternationalFormat());
        }
    }
}
