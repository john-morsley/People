namespace Morsley.UK.People.Persistence.Integration.Tests;

internal class Add : PersonRepositoryTests
{
    public Add() : base ("Add_Test") { }

    [Test]
    public async Task Adding_A_Person_Should_Result_In_That_Person_Being_Added()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        var person = GenerateTestPerson();

        var sut = new PersonRepository(MongoContext!);

        // Act...
        await sut.AddAsync(person);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(1);

        var added = GetPersonFromDatabase(person.Id);
        added.Should().BeEquivalentTo(person);
    }
}