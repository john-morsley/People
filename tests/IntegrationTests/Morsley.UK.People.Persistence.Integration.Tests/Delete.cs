namespace Morsley.UK.People.Persistence.Integration.Tests;

internal class Delete : PersonRepositoryTests
{
    [Test]
    public async Task Deleting_A_Person_Should_Result_In_That_Person_Being_Deleted()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        var person = GenerateTestPerson();
        AddPersonToDatabase(person);

        NumberOfPeopleInDatabase().Should().Be(1);

        var sut = new PersonRepository(MongoContext!);

        // Act...
        await sut.DeleteAsync(person.Id);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(0);
    }
}