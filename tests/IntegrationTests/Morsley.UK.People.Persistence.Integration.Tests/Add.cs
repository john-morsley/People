namespace Morsley.UK.People.Persistence.Integration.Tests;

internal class Add : PersonRepositoryTests
{
    public Add() : base ("Add_Test") { }

    [Test]
    public async Task Adding_A_Person_Should_Result_In_That_Person_Being_Added()
    {
        // Arrange...
        NumberOfPeople().Should().Be(0);

        var person = GeneratePerson();

        var sut = new PersonRepository(MongoContext!);

        // Act...
        await sut.AddAsync(person);

        // Assert...
        NumberOfPeople().Should().Be(1);

        var added = GetPersonFromDatabase(person.Id);
        added.Should().BeEquivalentTo(person);
    }
}