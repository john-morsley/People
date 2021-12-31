namespace UserRepositoryTests;

internal class Get : UserRepositoryTests
{
    [Test]
    public async Task Getting_A_User_Should_Result_In_That_User_Being_Returned()
    {
        // Arrange...
        var sut = new UserRepository(_mongoContext);
        var userId = Guid.NewGuid();
        var user = GenerateTestUser(userId);
        AddUserToDatabase(user);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        var result = await sut.GetByIdAsync(userId);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        result.Id.Should().Be(userId);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_No_Pagination_Options_Should_Result_In_A_Page_Of_All_Users_Being_Returned()
    {
        // Arrange...
        var sut = new UserRepository(_mongoContext);
        AddUsersToDatabase(10);
        NumberOfUsersInDatabase().Should().Be(10);

        // Act...
        var getUsersOptions = new GetOptions();
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(10);
        pageOfUsers.Count.Should().Be(10);
        pageOfUsers.Count().Should().Be(10);
        pageOfUsers.CurrentPage.Should().Be(1);
        pageOfUsers.TotalPages.Should().Be(1);
        pageOfUsers.TotalCount.Should().Be(10);
        pageOfUsers.HasPrevious.Should().BeFalse();
        pageOfUsers.HasNext.Should().BeFalse();
        var usersInDatabase = GetUsersFromDatabase().ToList();
        var firstUserOnPage = pageOfUsers.First();
        var lastUserOnPage = pageOfUsers.Last();
        var firstUserInDatabase = usersInDatabase.First();
        var lastUserInDatabase = usersInDatabase.Last();
        firstUserOnPage.Should().Equals(firstUserInDatabase);
        lastUserOnPage.Should().Equals(lastUserInDatabase);
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Pagination_Options_Should_Result_In_A_Page_Of_Users_Being_Returned()
    {
        // Arrange...
        var sut = new UserRepository(_mongoContext);
        AddUsersToDatabase(10);
        NumberOfUsersInDatabase().Should().Be(10);

        // Act...
        var getUsersOptions = new GetOptions();
        getUsersOptions.PageSize = 1;
        getUsersOptions.PageNumber = 1;
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(10);
        pageOfUsers.Count.Should().Be(1);
        var usersInDatabase = GetUsersFromDatabase().ToList();
        var firstUserOnPage = pageOfUsers.First();
        var lastUserOnPage = pageOfUsers.Last();
        var firstUserInDatabase = usersInDatabase.First();
        firstUserOnPage.Should().Equals(firstUserInDatabase);
        lastUserOnPage.Should().Equals(firstUserInDatabase);
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Pagination_Options_Should_Result_In_The_Correct_Page_Of_Users_Being_Returned()
    {
        // Arrange...
        var sut = new UserRepository(_mongoContext);
        AddUsersToDatabase(25);
        NumberOfUsersInDatabase().Should().Be(25);

        // Act...
        var getUsersOptions = new GetOptions();
        getUsersOptions.PageSize = 10;
        getUsersOptions.PageNumber = 2;
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(25);
        //pageOfUsers.Count.Should().Be(1);
        pageOfUsers.Count.Should().Be(10);
        pageOfUsers.CurrentPage.Should().Be(2);
        pageOfUsers.TotalPages.Should().Be(3);
        pageOfUsers.TotalCount.Should().Be(25);
        pageOfUsers.HasPrevious.Should().BeTrue();
        pageOfUsers.HasNext.Should().BeTrue();
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Search_Criteria_Should_Result_In_A_Page_Of_Users_That_Match_The_Search_Being_Returned()
    {
        // Arrange...
        var sut = new UserRepository(_mongoContext);
        AddUsersToDatabase(5);
        var expected = new Users.Domain.Models.User() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Morsley" };
        AddUserToDatabase(expected);
        AddUsersToDatabase(5);
        NumberOfUsersInDatabase().Should().Be(11);

        // Act...
        var getUsersOptions = new GetOptions();
        getUsersOptions.PageSize = 1;
        getUsersOptions.PageNumber = 1;
        getUsersOptions.Search = "orsle";
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(11);
        pageOfUsers.Count.Should().Be(1);
        var firstUserOnPage = pageOfUsers.First();
        firstUserOnPage.Should().Equals(expected);
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Filter_Criteria_Should_Result_In_A_Page_Of_Users_That_Match_The_Filter()
    {
        // Arrange...
        var sut = new UserRepository(_mongoContext);
        AddUsersToDatabase(5);
        var john = new Users.Domain.Models.User() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        AddUserToDatabase(john);
        var jane = new Users.Domain.Models.User() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe" };
        AddUserToDatabase(jane);
        AddUsersToDatabase(5);
        NumberOfUsersInDatabase().Should().Be(12);

        // Act...
        var getUsersOptions = new GetOptions();
        getUsersOptions.PageSize = 2;
        getUsersOptions.PageNumber = 1;
        getUsersOptions.AddFilter(new Filter("LastName", "Doe"));
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(12);
        pageOfUsers.Count.Should().Be(2);
        var firstUserOnPage = pageOfUsers.First();
        firstUserOnPage.Should().Equals(john);
        var secondUserOnPage = pageOfUsers.Skip(1).First();
        secondUserOnPage.Should().Equals(jane);
    }
}
