namespace UserRepositoryTests;

internal class Delete : UserRepositoryTests
{
    [Test]
    public async Task Deleting_A_User_Should_Result_In_That_User_Being_Deleted()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var user = GenerateTestUser();
        AddUserToDatabase(user);

        NumberOfUsersInDatabase().Should().Be(1);

        var sut = new UserRepository(_mongoContext);

        // Act...
        await sut.DeleteAsync(user.Id);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);
    }
}
