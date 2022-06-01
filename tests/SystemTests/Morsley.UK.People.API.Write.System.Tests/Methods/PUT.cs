namespace Morsley.UK.People.API.Write.Tests.Methods;

public class PUT_UpdatePerson : WriteApplicationTestFixture<WriteProgram>
{
    // If an attempt is made to update a user via PUT and the user exists, then the user should be updated and a 200 OK
    // HTTP status code should be returned.
    [Test]
    [Category("Happy")]
    public async Task Given_Person_Exists___When_Put_Update_Person___Then_200_OK_And_Person_Updated()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        var originalPerson = WriteDatabase.GeneratePerson();
        WriteDatabase.AddPerson(originalPerson);

        WriteDatabase.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/person/{originalPerson.Id}";
        var updatePersonRequest = GenerateTestUpdatePersonRequest(originalPerson.Id, originalPerson.Sex, originalPerson.Gender);
        var updatePersonRequestJson = System.Text.Json.JsonSerializer.Serialize(updatePersonRequest);
        var payload = new StringContent(updatePersonRequestJson, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient!.PutAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(1);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userResource = DeserializePersonResource(content);
        userResource.Should().NotBeNull();

        // - Person
        userResource!.Data.Should().NotBeNull();
        var actualPerson = WriteDatabase.GetPersonFromDatabase(originalPerson.Id);

        // The result of upsert should 'equal' the requested upsert...
        ObjectComparer.PublicInstancePropertiesEqual(userResource.Data!, updatePersonRequest).Should().BeTrue();

        //actualPerson.Should().NotBeEquivalentTo(originalPerson);
        // The result of upsert should 'equal' the upserted person in the database...
        ObjectComparer.PublicInstancePropertiesEqual(userResource.Data!, actualPerson, "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();
    }

    // If an attempt is made to update a user via PUT and the user does not exist, then the user should be created and
    // a 201 Created HTTP status code should be returned. In addition, the Location in the response Header should
    // contain a link to the newly created resource.
    [Test]
    [Category("Happy")]
    public async Task Given_Person_Does_Not_Exist___When_Put_Update_Person___Then_201_Created_And_Person_Created()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);
        ReadDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var personId = Guid.NewGuid();
        var url = $"/api/person/{personId}";
        var upsertPersonRequest = GenerateTestUpdatePersonRequest(personId);
        var upsertPersonRequestJson = System.Text.Json.JsonSerializer.Serialize(upsertPersonRequest);
        var payload = new StringContent(upsertPersonRequestJson, System.Text.Encoding.UTF8, "application/json");
        var result = await HttpClient!.PutAsync(url, payload);

        // Assert...
        using (new AssertionScope())
        {
            WriteDatabase.NumberOfPeople().Should().Be(1);

            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.Created);

            var content = await result.Content.ReadAsStringAsync();
            content.Length.Should().BeGreaterThan(0);

            var personResource = DeserializePersonResource(content);
            personResource.Should().NotBeNull();

            // - Person
            personResource!.Data.Should().NotBeNull();
            ObjectComparer.PublicInstancePropertiesEqual(personResource.Data!, upsertPersonRequest).Should().BeTrue();

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
        }
    }

    // 
    [Test]
    [Category("Unhappy")]
    public async Task When_Put_Invalid_Data___Then_400_BadRequest_And_Errors_Object_Should_Detail_Issues()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var personId = Guid.NewGuid();
        var url = $"/api/person/{personId}";
        var updatePersonRequestJson = System.Text.Json.JsonSerializer.Serialize("");
        var payload = new StringContent(updatePersonRequestJson, System.Text.Encoding.UTF8, "application/json");
        var httpResponse = await HttpClient!.PutAsync(url, payload);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await httpResponse.Content.ReadAsStringAsync();
        // ToDo --> Validate error object

    }

    [Test]
    [Category("Unhappy")]
    public async Task When_Put_Invalid_Update_Person___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var personId = Guid.NewGuid();
        var url = $"/api/person/{personId}";
        var updatePersonRequest = GenerateTestUpdatePersonRequest(personId);
        updatePersonRequest.FirstName = string.Empty;
        updatePersonRequest.LastName = string.Empty;
        var updatePersonRequestJson = System.Text.Json.JsonSerializer.Serialize(updatePersonRequest);
        var payload = new StringContent(updatePersonRequestJson, System.Text.Encoding.UTF8, "application/json");
        var result = await HttpClient!.PutAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var content = await result.Content.ReadAsStringAsync();
        var validationErrors = new Dictionary<string, string>
        {
            { "FirstName", "First name cannot be empty. | First name cannot exceed 100 characters." },
            { "LastName", "Last name cannot be empty. | Last name cannot exceed 100 characters." }
        };
        ProblemDetailsShouldContainValidationIssues(content, validationErrors);
    }
}
