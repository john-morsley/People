namespace UserRepositoryTests;

internal class Update : UserRepositoryTests
{
    [Test]
    public async Task Updating_A_User_Should_Result_In_That_User_Being_Updated()
    {
        // Arrange...
        var sut = new UserRepository(_mongoContext);
        var userId = Guid.NewGuid();
        var existing = GenerateTestUser(userId);
        AddUserToDatabase(existing);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        existing.FirstName = _autoFixture.Create<string>();
        existing.LastName = _autoFixture.Create<string>();
        await sut.UpdateAsync(existing);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        var updated = GetUserFromDatabase(userId);
        updated.Id.Should().Be(userId);
        updated.FirstName.Should().Be(existing.FirstName);
        updated.LastName.Should().Be(existing.LastName);
    }
}
