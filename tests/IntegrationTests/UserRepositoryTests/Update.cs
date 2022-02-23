namespace UserRepositoryTests;

internal class Update : UserRepositoryTests
{
    [Test]
    public async Task Updating_A_User_Should_Result_In_That_User_Being_Updated()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        var existing = GenerateTestUser();
        AddUserToDatabase(existing);

        NumberOfUsersInDatabase().Should().Be(1);

        var sut = new UserRepository(_mongoContext);

        // Act...
        existing.FirstName = _autoFixture.Create<string>();
        existing.LastName = _autoFixture.Create<string>();
        await sut.UpdateAsync(existing);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        var updated = GetUserFromDatabase(existing.Id);
        updated.Id.Should().Be(existing.Id);
        updated.Should().BeEquivalentTo(existing);
    }
}
