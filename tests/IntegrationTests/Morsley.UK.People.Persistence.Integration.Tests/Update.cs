namespace Morsley.UK.People.Persistence.Integration.Tests;

internal class Update : PersonRepositoryTests
{
    public Update() : base("Update_Test") { }

    [Test]
    public async Task Updating_A_Person_Should_Result_In_That_Person_Being_Updated()
    {
        // Arrange...
        NumberOfPeople().Should().Be(0);

        var existing = GeneratePerson();
        AddPersonToDatabase(existing);

        NumberOfPeople().Should().Be(1);

        var sut = new PersonRepository(MongoContext!);

        // Act...
        existing.FirstName = AutoFixture.Create<string>();
        existing.LastName = AutoFixture.Create<string>();
        await sut.UpdateAsync(existing);

        // Assert...
        NumberOfPeople().Should().Be(1);

        var updated = GetPersonFromDatabase(existing.Id);
        updated.Id.Should().Be(existing.Id);
        updated.Should().BeEquivalentTo(existing);
    }
}