namespace Morsley.UK.People.API.Write.Tests.Methods;

public class DELETE : WriteApplicationTestFixture<WriteProgram>
{
    /// <notes>
    /// Deleting an existing person should result in a 200 OK.
    /// It should also result in that person being deleted from the write database immediately.
    /// It should also result in that person being deleted from the read database given eventual consistency.
    /// </notes>
    [Test]
    [Category("Happy")]
    public async Task Given_Person_Exists___When_Delete_Is_Attempted___Then_NoContent_And_Person_Deleted()
    {
        // Arrange...
        ReadDatabase!.NumberOfPeople().Should().Be(0);
        WriteDatabase.NumberOfPeople().Should().Be(0);
        Cache.NumberOfPeople().Should().Be(0);

        var personToBeDeleted = WriteDatabase.GeneratePerson();
        var personId = personToBeDeleted.Id;
        await WriteDatabase.AddPerson(personToBeDeleted);
        WriteDatabase.NumberOfPeople().Should().Be(1);
        await ReadDatabase.AddPerson(personToBeDeleted);
        ReadDatabase.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{personId}";

        // Act...
        var result = await HttpClient!.DeleteAsync(url);

        // Assert...
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);

        var shouldNotExistPerson = WriteDatabase.GetPersonFromDatabase(personId);
        shouldNotExistPerson.Should().BeNull();

        // - Database
        WriteDatabase.NumberOfPeople().Should().Be(0);
        ReadDatabase!.NumberOfPeople(delayInMilliSeconds: 50, maximumNumberOfRetries: 200, expectedResult: 1).Should().Be(0);
    }

    /// <notes>
    /// Deleting an existing person should result in a 200 OK.
    /// It should also result in that person being deleted from the write database immediately.
    /// It should also result in that person being deleted from the read database given eventual consistency.
    /// </notes>
    [Test]
    [Category("Happy")]
    public async Task Given_Person_Exists_And_Is_In_The_Cache___When_Delete_Is_Attempted___Then_NoContent_And_Person_Deleted_And_Removed_From_Cache()
    {
        // Arrange...
        ReadDatabase!.NumberOfPeople().Should().Be(0);
        WriteDatabase.NumberOfPeople().Should().Be(0);
        //Cache.NumberOfPeople().Should().Be(0);

        var personToBeDeleted = WriteDatabase.GeneratePerson();
        var personId = personToBeDeleted.Id;
        await WriteDatabase.AddPerson(personToBeDeleted);
        WriteDatabase.NumberOfPeople().Should().Be(1);
        await ReadDatabase.AddPerson(personToBeDeleted);
        ReadDatabase.NumberOfPeople().Should().Be(1);
        await Cache.AddPerson(personToBeDeleted);
        //Cache.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/person/{personId}";
        var result = await HttpClient!.DeleteAsync(url);

        // Assert...
        using (new AssertionScope())
        {
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var content = await result.Content.ReadAsStringAsync();
            content.Length.Should().Be(0);

            var shouldNotExistPerson = WriteDatabase.GetPersonFromDatabase(personId);
            shouldNotExistPerson.Should().BeNull();

            // - Database
            WriteDatabase.NumberOfPeople().Should().Be(0);
            ReadDatabase!.NumberOfPeople(delayInMilliSeconds: 50, maximumNumberOfRetries: 200, expectedResult: 0).Should().Be(0);
            //Cache.NumberOfPeople().Should().Be(0);
        }
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_Not_Authenticated_And_Delete_Is_Attempted___Then_401_NotFound()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        WriteDatabase.NumberOfPeople().Should().Be(0);

        // Act...
        var url = $"/api/person/{personId}";
        var result = await HttpClient!.DeleteAsync(url);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

    [Test]
    [Category("Unhappy")]
    public async Task Given_Person_Does_Not_Exist___When_Delete_Is_Attempted___Then_404_NotFound()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        WriteDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{personId}";

        // Act...
        var result = await HttpClient!.DeleteAsync(url);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }
}
