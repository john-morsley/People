namespace Morsley.UK.People.API.Write.Tests.Methods;

public class PUT_UpdatePerson : SecuredApplicationTestFixture<WriteProgram>
{
    // If an attempt is made to update a user via PUT and the user exists, then the user should be updated and a 200 OK
    // HTTP status code should be returned.
    [Test]
    [Category("Happy")]
    public async Task Given_User_Exists___When_Put_Update_User___Then_200_OK_And_User_Updated()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        var originalUser = GenerateTestPerson();
        AddPersonToDatabase(originalUser);

        NumberOfPeopleInDatabase().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        // Act...
        var url = $"/api/person/{originalUser.Id}";
        var updateUserRequest = GenerateTestUpdatePersonRequest(originalUser.Id, originalUser.Sex, originalUser.Gender);
        var updateUserRequestJson = System.Text.Json.JsonSerializer.Serialize(updateUserRequest);
        var payload = new StringContent(updateUserRequestJson, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient!.PutAsync(url, payload);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(1);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var userResource = DeserializePersonResource(content);
        userResource.Should().NotBeNull();

        // - Person
        userResource!.Data.Should().NotBeNull();
        var actualUser = GetPersonFromDatabase(originalUser.Id);
        
        // The result of upsert should 'equal' the requested upsert...
        ObjectComparer.PublicInstancePropertiesEqual(userResource.Data!, updateUserRequest).Should().BeTrue();

        //actualUser.Should().NotBeEquivalentTo(originalUser);
        // The result of upsert should 'equal' the upserted person in the database...
        ObjectComparer.PublicInstancePropertiesEqual(userResource.Data!, actualUser, "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();
    }

    // If an attempt is made to update a user via PUT and the user does not exist, then the user should be created and
    // a 201 Created HTTP status code should be returned. In addition, the Location in the response Header should
    // contain a link to the newly created resource.
    [Test]
    [Category("Happy")]
    public async Task Given_User_Does_Not_Exist___When_Put_Update_User___Then_201_Created_And_User_Created()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var userId = Guid.NewGuid();
        var url = $"/api/person/{userId}";
        var upsertUserRequest = GenerateTestUpdatePersonRequest(userId);
        var upsertUserRequestJson = System.Text.Json.JsonSerializer.Serialize(upsertUserRequest);
        var payload = new StringContent(upsertUserRequestJson, System.Text.Encoding.UTF8, "application/json");
        var result = await HttpClient!.PutAsync(url, payload);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(1);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var personResource = DeserializePersonResource(content);
        personResource.Should().NotBeNull();
        
        // - User
        personResource!.Data.Should().NotBeNull();
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data!, upsertUserRequest).Should().BeTrue();

        // - Links
        personResource.Links.Should().NotBeNull();
        personResource.Links!.Count.Should().Be(2);
        LinksForPersonShouldBeCorrect(personResource.Links, personResource.Data!.Id);

        // - Embedded
        personResource.Embedded.Should().BeNull();

        // - Headers
        result.Headers.Location.Should().Be($"https://localhost/api/person/{userId}");

        // - Database
        var actualUser = GetPersonFromDatabase(userId);
        actualUser.Should().NotBeNull();
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualUser, "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();
    }

    // 
    [Test]
    [Category("Unhappy")]
    public async Task When_Put_Invalid_Data___Then_400_BadRequest_And_Errors_Object_Should_Detail_Issues()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var userId = Guid.NewGuid();
        var url = $"/api/person/{userId}";
        var updateUserRequestJson = System.Text.Json.JsonSerializer.Serialize("");
        var payload = new StringContent(updateUserRequestJson, System.Text.Encoding.UTF8, "application/json");
        var httpResponse = await HttpClient!.PutAsync(url, payload);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await httpResponse.Content.ReadAsStringAsync();
        // ToDo --> Validate error object

        //userResponseJson.Length.Should().BeGreaterThan(0);
        //var userResponse = DeserializeUser(userResponseJson);
        //userResponse.Should().NotBeNull();
        //var actualUser = GetPersonFromDatabase(userResponse.Id);
        //httpResponse.Headers.Location.Should().Be($"http://localhost/api/v1/users/{userResponse.Id}");
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_Put_Invalid_Update_User___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        // Act...
        var userId = Guid.NewGuid();
        var url = $"/api/person/{userId}";
        var updateUserRequest = GenerateTestUpdatePersonRequest(userId);
        updateUserRequest.FirstName = string.Empty;
        updateUserRequest.LastName = string.Empty;
        var updateUserRequestJson = System.Text.Json.JsonSerializer.Serialize(updateUserRequest);
        var payload = new StringContent(updateUserRequestJson, System.Text.Encoding.UTF8, "application/json");
        var result = await HttpClient!.PutAsync(url, payload);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(0);

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
