namespace UserRepositoryTests;

internal class Delete : UserRepositoryTests
{
    [Test]
    public async Task Deleting_A_User_Should_Result_In_That_User_Being_Deleted()
    {
        // Arrange...
        var sut = new UserRepository(_mongoContext);
        var userId = Guid.NewGuid();
        var user = GenerateTestUser(userId);
        AddUserToDatabase(user);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        await sut.DeleteAsync(userId);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);
    }
}
