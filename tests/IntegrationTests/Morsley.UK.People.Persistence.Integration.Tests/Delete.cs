namespace Morsley.UK.People.Persistence.Integration.Tests;

internal class Delete : PersonRepositoryTests
{
    public Delete() : base("Delete_Test") { }

    [Test]
    public async Task Deleting_A_Person_Should_Result_In_That_Person_Being_Deleted()
    {
        // Arrange...
        NumberOfPeople().Should().Be(0);

        var person = GeneratePerson();
        await AddPerson(person);

        NumberOfPeople().Should().Be(1);

        var sut = new PersonRepository(MongoContext!);

        // Act...
        await sut.DeleteAsync(person.Id);

        // Assert...
        NumberOfPeople().Should().Be(0);
    }
}