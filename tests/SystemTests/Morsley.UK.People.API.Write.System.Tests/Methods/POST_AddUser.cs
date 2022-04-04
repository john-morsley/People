namespace Morsley.UK.People.API.Write.Tests.Methods;

public class POST_AddPerson : WriteApplicationTestFixture<WriteProgram>
{
    //[SetUp]
    //protected async override Task SetUp()
    //{
    //    await base.SetUp();
    //}

    /// <notes>
    /// Adding a valid person should result in a 200 OK.
    /// It should also result in that person being added to the write database immediately.
    /// It should also result in that person being added to the read database given eventual consistency.
    /// </notes>
    [Test]
    [Category("Happy")]
    public async Task Given_Person_Does_Not_Exist___When_Post_Add_Person___Then_200_OK_And_Person_Should_Be_Created()
    {
        // Arrange...
        ReadDatabase!.NumberOfPeople().Should().Be(0);
        WriteDatabase!.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        const string url = "/api/person";
        var addPersonRequest = GenerateAddPersonRequest();
        var addPersonRequestJson = System.Text.Json.JsonSerializer.Serialize(addPersonRequest);
        var payload = new StringContent(addPersonRequestJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var result = await HttpClient!.PostAsync(url, payload);

        // Assert...
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var personResource = DeserializePersonResource(content);
        personResource.Should().NotBeNull();

        // - Person
        personResource!.Data.Should().NotBeNull();
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data!, addPersonRequest, "Id").Should().BeTrue();

        // - Links
        personResource.Links.Should().NotBeNull();
        personResource.Links!.Count.Should().Be(2);
        LinksForPersonShouldBeCorrect(personResource.Links, personResource.Data!.Id);

        // - Embedded
        personResource.Embedded.Should().BeNull();

        // - Headers
        result.Headers.Location.Should().Be($"http://localhost/api/person/{personResource.Data.Id}");

        // - Databases
        ReadDatabase!.NumberOfPeople(delayInMilliSeconds: 50, maximumNumberOfRetries: 200, expectedResult: 1).Should().Be(1);
        WriteDatabase!.NumberOfPeople().Should().Be(1);

        // Verify that the person in the write database is what we expect it to be ...
        var actualWritePerson = WriteDatabase.GetPersonFromDatabase(personResource.Data.Id);
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualWritePerson, "Id", "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();

        // Verify that the person in the read database is what we expect it to be ...
        var actualReadPerson = WriteDatabase.GetPersonFromDatabase(personResource.Data.Id);
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualReadPerson, "Id", "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();

        // Verify that both read and write instances are equal...
        actualReadPerson.Should().BeEquivalentTo(actualWritePerson);
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_Post_Invalid_Data___Then_400_BadRequest_And_Errors_Object_Should_Detail_Issues()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = "/api/person/";
        var addUserRequestJson = System.Text.Json.JsonSerializer.Serialize("");
        var payload = new StringContent(addUserRequestJson, System.Text.Encoding.UTF8, "application/json");
        var result = await HttpClient.PostAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(0);
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await result.Content.ReadAsStringAsync();
        // ToDo --> Validate error object

        //userResponseJson.Length.Should().BeGreaterThan(0);
        //var userResponse = DeserializeUser(userResponseJson);
        //userResponse.Should().NotBeNull();
        //var actualUser = GetPersonFromDatabase(userResponse.Id);
        //httpResponse.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userResponse.Id}");
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_Post_Invalid_Add_User___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = "/api/person/";
        var addUserRequest = new Morsley.UK.People.API.Contracts.Requests.AddPersonRequest();
        var addUserRequestJson = System.Text.Json.JsonSerializer.Serialize(addUserRequest);
        var payload = new StringContent(addUserRequestJson, System.Text.Encoding.UTF8, "application/json");
        var result = await HttpClient!.PostAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var content = await result.Content.ReadAsStringAsync();
        var validationErrors = new Dictionary<string, string>
        {
            { "FirstName", "First name cannot be empty." },
            { "LastName", "Last name cannot be empty." }
        };
        ProblemDetailsShouldContainValidationIssues(content, validationErrors);
    }

    //[Test]
    //[Category("Unhappy")]
    //public async Task When_An_Unsupported_Method_Is_Called___Then_405_MethodNotAllowed()
    //{
    //    // Arrange...
    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

    //    await AuthenticateAsync(Username, Password);

    //    // Act...
    //    var dodgyUserId = Guid.NewGuid;
    //    var url = $"/api/person/{dodgyUserId}"; // POST with a user ID not allowed!
    //    var payload = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
    //    var result = await HttpClient!.PostAsync(url, payload);

    //    // Assert...
    //    result.IsSuccessStatusCode.Should().BeFalse();
    //    result.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    //    var content = await result.Content.ReadAsStringAsync();
    //    content.Length.Should().Be(0);
    //}
}
