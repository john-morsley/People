namespace UserRepositoryTests;

internal class Add : UserRepositoryTests
{
    [Test]
    public async Task Adding_A_User_Should_Result_In_That_User_Being_Added()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var user = GenerateTestUser();

        var sut = new UserRepository(_mongoContext);

        // Act...
        await sut.AddAsync(user);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        var added = GetUserFromDatabase(user.Id);
        added.Should().BeEquivalentTo(user);
    }
}
