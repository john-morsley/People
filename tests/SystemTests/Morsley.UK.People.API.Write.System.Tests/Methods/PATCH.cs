namespace Morsley.UK.People.API.Write.Tests.Methods;

// Patch Operations we can test:
// - Add 
// - Remove? What will this do? Will it set that value to its default?
// - Replace - This should change the objects property value (equivalent to an update).
// - Copy? Will this copy an objects property to another property. i.e. FirstName to LastName
// - Move? As the copy above, but the source property will then be set to its default value.
// - Test? Tests that an objects property value is equal to a specified value.

// Refer to: http://jsonpatch.com/

public class PATCH_PartiallyUpdatePerson : WriteApplicationTestFixture<WriteProgram>
{
    [Test]
    [Category("Happy")]
    public async Task Given_Person_Exists___When_Replace_Patch_Partial_Update_Person___Then_200_OK_And_Person_Partially_Updated()
    {
        // Arrange...
        ReadDatabase!.NumberOfPeople().Should().Be(0);
        WriteDatabase.NumberOfPeople().Should().Be(0);

        var billGates = new Person
        {
            FirstName = "Bill",
            LastName = "Gates",
            Sex = Sex.Male,
            Gender = Gender.Cisgender,
            DateOfBirth = new DateTime(1955, 10, 28, 0, 0, 0, DateTimeKind.Utc)
        };
        ReadDatabase.AddPersonToDatabase(billGates);
        WriteDatabase.AddPersonToDatabase(billGates);

        ReadDatabase!.NumberOfPeople().Should().Be(1);
        WriteDatabase.NumberOfPeople().Should().Be(1);

        var replacementFirstName = "William";

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{billGates.Id}";
        var updates = new JsonPatchDocument<PartiallyUpdatePersonRequest>();
        updates.Replace(_ => _.FirstName, replacementFirstName);
        var partiallyUpdatePersonJson = JsonConvert.SerializeObject(updates);
        var payload = new StringContent(partiallyUpdatePersonJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var httpResponse = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        var actualPerson = WriteDatabase.GetPersonFromDatabase(billGates.Id);
        actualPerson.Should().NotBeNull();
        actualPerson.FirstName.Should().Be(replacementFirstName);

        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var personResource = DeserializePersonResource(content);
        personResource.Should().NotBeNull();

        // - Person
        personResource!.Data.Should().NotBeNull();
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data!, actualPerson, "Emails", "Phones", "Addresses", "Created", "Updated").Should().BeTrue();

        // - Links
        personResource.Links.Should().NotBeNull();
        personResource.Links!.Count.Should().Be(3);
        LinksForPersonShouldBeCorrect(personResource.Links, personResource.Data!.Id);

        // - Embedded
        personResource.Embedded.Should().BeNull();

        ObjectComparer.PublicInstancePropertiesEqual(billGates, actualPerson, "FirstName", "Addresses", "Emails", "Phones", "Created", "Updated");

        // - Databases
        ReadDatabase!.NumberOfPeople().Should().Be(1);
        WriteDatabase!.NumberOfPeople().Should().Be(1);

        ObjectComparer.PublicInstancePropertiesEqual(billGates, actualPerson, "FirstName", "Sex", "Gender", "DateOfBirth", "Addresses", "Emails", "Phones", "Created", "Updated");

        // Verify that the person in the write database is what we expect it to be ...
        var actualWritePerson = WriteDatabase.GetPersonFromDatabase(personResource.Data.Id);
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualWritePerson, "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();


        // Verify that the person in the read database is what we expect it to be ...
        Person actualReadPerson = null;
        for (var i = 0; i < 50; i++)
        {
            actualReadPerson = ReadDatabase.GetPersonFromDatabase(personResource.Data.Id);
            if (actualReadPerson.FirstName == replacementFirstName) break;
            await Task.Delay(25);
        }
        if (actualReadPerson is null) Assert.Fail("Could not get actual read person!");
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualReadPerson, "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();

        // Verify that both read and write instances are equal...
        actualReadPerson.Should().BeEquivalentTo(actualWritePerson);

        // Will produce the following JSON Patch Document:
        /*
         
        [
            {
                "value":"William",
                "path":"/FirstName",
                "op":"replace"
            }
        ]

        */
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Person_Exists___When_Remove_Patch_Partial_Update_Person___Then_200_OK_And_Person_Partially_Updated()
    {
        // Arrange...
        ReadDatabase!.NumberOfPeople().Should().Be(0);
        WriteDatabase.NumberOfPeople().Should().Be(0);

        var personToBeUpdated = WriteDatabase.GeneratePerson();
        personToBeUpdated.Sex = Sex.Male;
        ReadDatabase.AddPersonToDatabase(personToBeUpdated);
        WriteDatabase.AddPersonToDatabase(personToBeUpdated);

        ReadDatabase!.NumberOfPeople().Should().Be(1);
        WriteDatabase.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{personToBeUpdated.Id}";
        var updates = new JsonPatchDocument<PartiallyUpdatePersonRequest>();
        updates.Remove(_ => _.Gender);
        updates.Remove(_ => _.Sex);
        updates.Remove(_ => _.DateOfBirth);
        var partiallyUpdatePersonJson = JsonConvert.SerializeObject(updates);
        var payload = new StringContent(partiallyUpdatePersonJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var httpResponse = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        var actualPerson = WriteDatabase.GetPersonFromDatabase(personToBeUpdated.Id);
        actualPerson.Should().NotBeNull();
        actualPerson.Gender.Should().BeNull();
        actualPerson.Sex.Should().BeNull();
        actualPerson.DateOfBirth.Should().BeNull();

        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var personResource = DeserializePersonResource(content);
        personResource.Should().NotBeNull();

        // - Person
        personResource!.Data.Should().NotBeNull();
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data!, actualPerson, "Emails", "Phones", "Addresses", "Created", "Updated").Should().BeTrue();

        // - Links
        personResource.Links.Should().NotBeNull();
        personResource.Links!.Count.Should().Be(3);
        LinksForPersonShouldBeCorrect(personResource.Links, personResource.Data!.Id);

        // - Embedded
        personResource.Embedded.Should().BeNull();

        // - Databases
        ReadDatabase!.NumberOfPeople().Should().Be(1);
        WriteDatabase!.NumberOfPeople().Should().Be(1);

        ObjectComparer.PublicInstancePropertiesEqual(personToBeUpdated, actualPerson, "Sex", "Gender", "DateOfBirth", "Addresses", "Emails", "Phones", "Created", "Updated");

        // Verify that the person in the write database is what we expect it to be ...
        var actualWritePerson = WriteDatabase.GetPersonFromDatabase(personResource.Data.Id);
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualWritePerson, "Id", "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();

        // Verify that the person in the read database is what we expect it to be ...
        Person actualReadPerson = null;
        for (var i = 0; i < 50; i++)
        {
            actualReadPerson = ReadDatabase.GetPersonFromDatabase(personResource.Data.Id);
            if (actualReadPerson.Gender is null &&
                actualReadPerson.Sex is null &&
                actualReadPerson.DateOfBirth is null) break;
            await Task.Delay(25);
        }
        if (actualReadPerson is null) Assert.Fail("Could not get actual read person!");
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualReadPerson, "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();

        // Verify that both read and write instances are equal...
        actualReadPerson.Should().BeEquivalentTo(actualWritePerson);

        // Will produce the following JSON Patch Document:
        /*
         
        [
            {
                "path":"/Gender",
                "op":"remove"
            },
            {
                "path":"/Sex",
                "op":"remove"
            },
            {
                "path":"/DateOfBirth",
                "op":"remove"
            }
        ]

        */
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Person_Exists___When_Add_Patch_Partial_Update_Person___Then_200_OK_And_Person_Partially_Updated()
    {
        // Arrange...
        ReadDatabase!.NumberOfPeople().Should().Be(0);
        WriteDatabase.NumberOfPeople().Should().Be(0);

        var alanTuring = new Person { FirstName = "Alan", LastName = "Turing", Created = DateTime.UtcNow };
        ReadDatabase.AddPersonToDatabase(alanTuring);
        WriteDatabase.AddPersonToDatabase(alanTuring);

        ReadDatabase!.NumberOfPeople().Should().Be(1);
        WriteDatabase.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{alanTuring.Id}";
        var updates = new JsonPatchDocument<PartiallyUpdatePersonRequest>();
        updates.Add(_ => _.Gender, Gender.Bigender);
        updates.Add(_ => _.Sex, Sex.Male);
        updates.Add(_ => _.DateOfBirth, "1912-06-23");
        var partiallyUpdatePersonJson = JsonConvert.SerializeObject(updates);
        var payload = new StringContent(partiallyUpdatePersonJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var result = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var personResource = DeserializePersonResource(content);
        personResource.Should().NotBeNull();

        // - Person
        personResource!.Data.Should().NotBeNull();

        // - Links
        personResource.Links.Should().NotBeNull();
        personResource.Links!.Count.Should().Be(3);
        LinksForPersonShouldBeCorrect(personResource.Links, personResource.Data!.Id);

        // - Embedded
        personResource.Embedded.Should().BeNull();

        // - Confirm that the update was successful
        var actualWritePerson = WriteDatabase.GetPersonFromDatabase(alanTuring.Id);
        actualWritePerson.Should().NotBeNull();
        actualWritePerson.Gender.Should().Be(Gender.Bigender);
        actualWritePerson.Sex.Should().Be(Sex.Male);
        actualWritePerson.DateOfBirth.Should().Be(new DateTime(1912, 06, 23, 0, 0, 0, DateTimeKind.Utc));

        // - Databases
        ReadDatabase!.NumberOfPeople().Should().Be(1);
        WriteDatabase!.NumberOfPeople().Should().Be(1);

        // Verify that the person in the write database is what we expect it to be...
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualWritePerson, "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();

        // Verify that the person in the read database is what we expect it to be...
        Person actualReadPerson = null;
        for (var i = 0; i < 50; i++)
        {
            actualReadPerson = ReadDatabase.GetPersonFromDatabase(personResource.Data.Id);
            if (actualReadPerson.Gender == Gender.Bigender &&
                actualReadPerson.Sex == Sex.Male &&
                actualReadPerson.DateOfBirth == new DateTime(1912, 6, 23, 0, 0, 0, DateTimeKind.Utc)) break;
            await Task.Delay(25);
        }
        if (actualReadPerson is null) Assert.Fail("Could not get actual read person!");
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualReadPerson, "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();

        // Verify that both read and write instances are equal...
        actualReadPerson.Should().BeEquivalentTo(actualWritePerson);

        // Will produce the following JSON Patch Document:
        /*
        
        [
            {
                "value":1,
                "path":"/Gender",
                "op":"add"
            },
            {
                "value":1,
                "path":"/Sex",
                "op":"add"
            },
            {
                "value":"1912-06-23",
                "path":"/DateOfBirth",
                "op":"add"
            }
        ]

        */
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Person_Exists___When_Copy_Patch_Partial_Update_Person___Then_200_OK_And_Person_Partially_Updated()
    {
        // Arrange...
        ReadDatabase!.NumberOfPeople().Should().Be(0);
        WriteDatabase.NumberOfPeople().Should().Be(0);

        var personToBeUpdated = WriteDatabase.GeneratePerson();
        personToBeUpdated.Updated = DateTime.UtcNow;
        ReadDatabase.AddPersonToDatabase(personToBeUpdated);
        WriteDatabase.AddPersonToDatabase(personToBeUpdated);

        ReadDatabase.NumberOfPeople().Should().Be(1);
        WriteDatabase.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{personToBeUpdated.Id}";
        var updates = new JsonPatchDocument<PartiallyUpdatePersonRequest>();
        updates.Copy(from => from.FirstName, to => to.LastName);
        var partiallyUpdatePersonJson = JsonConvert.SerializeObject(updates);
        var payload = new StringContent(partiallyUpdatePersonJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var httpResponse = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var personResource = DeserializePersonResource(content);
        personResource.Should().NotBeNull();

        // - Person
        personResource!.Data.Should().NotBeNull();

        // - Links
        personResource.Links.Should().NotBeNull();
        personResource.Links!.Count.Should().Be(3);
        LinksForPersonShouldBeCorrect(personResource.Links, personResource.Data!.Id);

        // - Embedded
        personResource.Embedded.Should().BeNull();

        // - Confirm that the update was successful
        var actualWritePerson = WriteDatabase.GetPersonFromDatabase(personToBeUpdated.Id);
        actualWritePerson.Should().NotBeNull();
        actualWritePerson.LastName.Should().Be(personToBeUpdated.FirstName);

        // - Databases
        ReadDatabase!.NumberOfPeople().Should().Be(1);
        WriteDatabase!.NumberOfPeople().Should().Be(1);

        // Verify that the person in the write database is what we expect it to be ...
        ObjectComparer.PublicInstancePropertiesEqual(personToBeUpdated, actualWritePerson, "LastName", "Addresses", "Emails", "Phones").Should().BeTrue();

        // Verify that the person in the read database is what we expect it to be ...
        Person actualReadPerson = null;
        for (var i = 0; i < 50; i++)
        {
            actualReadPerson = ReadDatabase.GetPersonFromDatabase(personResource.Data.Id);
            if (actualReadPerson.FirstName == actualReadPerson.LastName) break;
            await Task.Delay(25);
        }
        if (actualReadPerson is null) Assert.Fail("Could not get actual read person!");
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualReadPerson, "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();

        // Verify that both read and write instances are equal...
        actualReadPerson.Should().BeEquivalentTo(actualWritePerson);

        // Will produce the following JSON Patch Document:
        /*

        [
            {
                "path": "/LastName",
                "op": "copy",
                "from": "/FirstName"
            }
        ]

        */
    }

    [Test]
    [Category("Happy")]
    public async Task Given_Person_Does_Not_Exist___When_Patch_Partial_Update_Person___Then_201_Created_And_Person_Created()
    {
        // Arrange...
        ReadDatabase.NumberOfPeople().Should().Be(0);
        WriteDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        var personId = Guid.NewGuid();
        var url = $"/api/person/{personId}";
        var updates = new JsonPatchDocument<PartiallyUpdatePersonRequest>();
        updates.Add(_ => _.FirstName, "John");
        updates.Add(_ => _.LastName, "Doe");
        var partiallyUpdatePersonJson = JsonConvert.SerializeObject(updates);
        var payload = new StringContent(partiallyUpdatePersonJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var result = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var personResource = DeserializePersonResource(content);
        personResource.Should().NotBeNull();

        // - Person
        personResource!.Data.Should().NotBeNull();
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data!, new { Id = personId, FirstName = "John", LastName = "Doe" }, "Gender", "Sex", "DateOfBirth").Should().BeTrue();

        // - Links
        personResource.Links.Should().NotBeNull();
        personResource.Links!.Count.Should().Be(3);
        LinksForPersonShouldBeCorrect(personResource.Links, personResource.Data!.Id);

        // - Embedded
        personResource.Embedded.Should().BeNull();

        // - Headers
        result.Headers.Location.Should().Be($"https://localhost/api/person/{personId}");

        // - Databases
        WriteDatabase!.NumberOfPeople().Should().Be(1);
        ReadDatabase!.NumberOfPeople(delayInMilliSeconds: 50, maximumNumberOfRetries: 200, expectedResult: 1).Should().Be(1);

        // Verify that the person in the write database is what we expect it to be ...
        var actualWritePerson = WriteDatabase.GetPersonFromDatabase(personResource.Data.Id);
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualWritePerson, "Id", "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();

        // Verify that the person in the read database is what we expect it to be ...
        var actualReadPerson = WriteDatabase.GetPersonFromDatabase(personResource.Data.Id);
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualReadPerson, "Id", "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();

        // Verify that both read and write instances are equal...
        actualReadPerson.Should().BeEquivalentTo(actualWritePerson);

        /*

        [
            {
                "op":"add",
                "path":"/FirstName",
                "value":"John"
            },
            {
                "op":"add",
                "path":"/LastName",
                "value":"Doe"
            }
        ]

        */
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_Partial_Update_For_Property_That_Does_Not_Exist___Then_422_UnprocessableEntity_And_Problem_Details()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        WriteDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{personId}";
        var partiallyUpdatePersonJson = "[" +
            "{" +
                "\"op\":\"remove\"," +
                "\"path\":\"/ThisDoesNotExist\"" +
            "}" +
        "]";
        var payload = new StringContent(partiallyUpdatePersonJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var httpResponse = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var issues = new Dictionary<string, string>
        {
            { "Operation_1", "Op: 'remove' - Path: '/ThisDoesNotExist' - Value: '[None]'" },
        };
        ProblemDetailsShouldContain(content, "Unexpected error(s) occurred!", "See the errors field for details.", issues);
    }

    [Test]
    [Category("Unhappy")]
    public async Task Given_Person_Exists___When_Partial_Update_For_Property_That_Does_Not_Exist___Then_422_UnprocessableEntity_Problem_Details()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        var personToBeUpdated = WriteDatabase.GeneratePerson();
        WriteDatabase.AddPersonToDatabase(personToBeUpdated);

        WriteDatabase.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{personToBeUpdated.Id}";
        var partiallyUpdatePersonJson = "[" +
            "{" +
                "\"op\":\"remove\"," +
                "\"path\":\"/ThisDoesNotExist\"" +
            "}" +
        "]";
        var payload = new StringContent(partiallyUpdatePersonJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var httpResponse = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(1);

        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var issues = new Dictionary<string, string>
        {
            { "Operation_1", "Op: 'remove' - Path: '/ThisDoesNotExist' - Value: '[None]'" },
        };
        ProblemDetailsShouldContain(content, "Unexpected error(s) occurred!", "See the errors field for details.", issues);
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_An_Attempt_Is_Made_To_Update_A_Person_Without_Authorizing____Then_401_Unauthorized()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        WriteDatabase.NumberOfPeople().Should().Be(0);
        var url = $"/api/person/{personId}";
        var payload = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var httpResponse = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }
}
