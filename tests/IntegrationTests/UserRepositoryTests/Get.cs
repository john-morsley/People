namespace UserRepositoryTests;

internal class Get : UserRepositoryTests
{
    [Test]
    public async Task Getting_A_User_Should_Result_In_That_User_Being_Returned()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var expected = GenerateTestUser();
        AddUserToDatabase(expected);
        
        NumberOfUsersInDatabase().Should().Be(1);

        var sut = new UserRepository(_mongoContext);

        // Act...
        var actual = await sut.GetByIdAsync(expected.Id);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_No_Pagination_Options_Should_Result_In_A_Page_Of_All_Users_Being_Returned()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var users = AddUsersToDatabase(10);

        NumberOfUsersInDatabase().Should().Be(10);

        var sut = new UserRepository(_mongoContext);

        // Act...
        var getUsersOptions = new GetOptions();
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(10);

        pageOfUsers.Count().Should().Be(10);
        pageOfUsers.CurrentPage.Should().Be(1);
        pageOfUsers.TotalPages.Should().Be(1);
        pageOfUsers.TotalCount.Should().Be(10);
        pageOfUsers.HasPrevious.Should().BeFalse();
        pageOfUsers.HasNext.Should().BeFalse();

        ShouldBeEquivalentTo(pageOfUsers, users);
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Pagination_Options_Should_Result_In_A_Page_Of_Users_Being_Returned()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var users = AddUsersToDatabase(10);

        NumberOfUsersInDatabase().Should().Be(10);

        var sut = new UserRepository(_mongoContext);

        // Act...
        var getUsersOptions = new GetOptions
        {
            PageSize = 1,
            PageNumber = 1
        };
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(10);

        pageOfUsers.Count().Should().Be(1);
        pageOfUsers.CurrentPage.Should().Be(1);
        pageOfUsers.TotalPages.Should().Be(10);
        pageOfUsers.TotalCount.Should().Be(10);
        pageOfUsers.HasPrevious.Should().BeFalse();
        pageOfUsers.HasNext.Should().BeTrue();

        ShouldBeEquivalentTo(pageOfUsers, users);
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Pagination_Options_Should_Result_In_The_Correct_Page_Of_Users_Being_Returned()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        AddUsersToDatabase(10);

        var users = AddUsersToDatabase(10);

        AddUsersToDatabase(10);

        NumberOfUsersInDatabase().Should().Be(30);

        var sut = new UserRepository(_mongoContext);

        // Act...
        var getUsersOptions = new GetOptions
        {
            PageSize = 10,
            PageNumber = 2
        };
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(30);

        pageOfUsers.Count().Should().Be(10);
        pageOfUsers.CurrentPage.Should().Be(2);
        pageOfUsers.TotalPages.Should().Be(3);
        pageOfUsers.TotalCount.Should().Be(30);
        pageOfUsers.HasPrevious.Should().BeTrue();
        pageOfUsers.HasNext.Should().BeTrue();

        ShouldBeEquivalentTo(pageOfUsers, users);
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Search_Criteria_Should_Result_In_A_Page_Of_Users_That_Match_The_Search_Being_Returned()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        AddUsersToDatabase(5);

        var expected = new Users.Domain.Models.User(Guid.NewGuid(), "John", "Morsley");
        AddUserToDatabase(expected);

        AddUsersToDatabase(5);

        NumberOfUsersInDatabase().Should().Be(11);

        var sut = new UserRepository(_mongoContext);

        // Act...
        var getUsersOptions = new GetOptions
        {
            PageSize = 1,
            PageNumber = 1,
            Search = "orsle"
        };
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(11);

        pageOfUsers.Count().Should().Be(1);
        pageOfUsers.CurrentPage.Should().Be(1);
        pageOfUsers.TotalPages.Should().Be(1);
        pageOfUsers.TotalCount.Should().Be(1);
        pageOfUsers.HasPrevious.Should().BeFalse();
        pageOfUsers.HasNext.Should().BeFalse();

        ShouldBeEquivalentTo(pageOfUsers, new List<Users.Domain.Models.User>{ expected });
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Filter_Criteria_Should_Result_In_A_Page_Of_Users_That_Match_The_Filter()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        AddUsersToDatabase(5);

        var john = new Users.Domain.Models.User(Guid.NewGuid(), "John", "Doe");
        AddUserToDatabase(john);

        var jane = new Users.Domain.Models.User(Guid.NewGuid(), "Jane", "Doe");
        AddUserToDatabase(jane);

        AddUsersToDatabase(5);

        NumberOfUsersInDatabase().Should().Be(12);

        var sut = new UserRepository(_mongoContext);

        // Act...
        var getUsersOptions = new GetOptions
        {
            PageSize = 2,
            PageNumber = 1
        };
        getUsersOptions.AddFilter(new Filter("LastName", "Doe"));
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(12);

        //pageOfUsers.Count().Should().Be(2);
        //var firstUserOnPage = pageOfUsers.First();
        //firstUserOnPage.Should().BeEquivalentTo(john);
        //var secondUserOnPage = pageOfUsers.Skip(1).First();
        //secondUserOnPage.Should().BeEquivalentTo(jane);
        ShouldBeEquivalentTo(pageOfUsers, new List<Users.Domain.Models.User> { john, jane });
    }

    private void ShouldBeEquivalentTo(
        Users.Domain.Interfaces.IPagedList<Users.Domain.Models.User> pageOfUsers, 
        IList<Users.Domain.Models.User> users)
    {
        foreach (var pagedUser in pageOfUsers)
        {
            var user = users.Single(_ => _.Id == pagedUser.Id);
            pagedUser.Should().BeEquivalentTo(user);
        }
    }
}
