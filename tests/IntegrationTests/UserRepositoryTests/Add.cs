namespace UserRepositoryTests;

internal class Add : UserRepositoryTests
{
    [Test]
    public async Task Adding_A_User_Should_Result_In_That_User_Being_Added()
    {
        // Arrange...
        var sut = new UserRepository(_mongoContext);            
        var userId = Guid.NewGuid();
        var user = GenerateTestUser(userId);

        // Act...
        await sut.CreateAsync(user);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        var added = GetUserFromDatabase(userId);
        added.Id.Should().Be(userId);
        added.FirstName.Should().Be(user.FirstName);
        added.LastName.Should().Be(user.LastName);
    }
}
