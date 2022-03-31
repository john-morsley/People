using Morsley.UK.People.Domain.Models;

namespace Morsley.UK.People.API.Write.Tests.Methods;

// Patch Operations we can test:
// - Add 
// - Remove? What will this do? Will it set that value to its default?
// - Replace - This should change the objects property value (equivalent to an update).
// - Copy? Will this copy an objects property to another property. i.e. FirstName to LastName
// - Move? As the copy above, but the source property will then be set to its default value.
// - Test? Tests that an objects property value is equal to a specfied value.

// Refer to: http://jsonpatch.com/

public class PATCH_PartiallyUpdatePerson : WriteApplicationTestFixture<WriteProgram>
{
    [Test]
    [Category("Happy")]
    public async Task Given_User_Exists___When_Replace_Patch_Partial_Update_User___Then_200_OK_And_User_Partially_Updated()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        var billGates = new Person
        {
            FirstName = "Bill",
            LastName = "Gates",
            Sex = Sex.Male,
            Gender = Gender.Cisgender,
            DateOfBirth = new DateTime(1955, 10, 28, 0, 0, 0, DateTimeKind.Utc)
        };
        WriteDatabase.AddPersonToDatabase(billGates);

        WriteDatabase.NumberOfPeople().Should().Be(1);

        var replacementFirstName = "William";

        WriteDatabase.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{billGates.Id}";
        var updates = new JsonPatchDocument<PartiallyUpdatePersonRequest>();
        updates.Replace(_ => _.FirstName, replacementFirstName);
        var partiallyUpdateUserJson = JsonConvert.SerializeObject(updates);
        var payload = new StringContent(partiallyUpdateUserJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var httpResponse = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(1);

        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var actualUser = WriteDatabase.GetPersonFromDatabase(billGates.Id);
        actualUser.Should().NotBeNull();
        actualUser.FirstName.Should().Be(replacementFirstName);

        ObjectComparer.PublicInstancePropertiesEqual(billGates, actualUser, "FirstName", "Addresses", "Emails", "Phones", "Created", "Updated");

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
    public async Task Given_User_Exists___When_Remove_Patch_Partial_Update_User___Then_200_OK_And_User_Partially_Updated()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        var personToBeUpdated = WriteDatabase.GenerateTestPerson();
        personToBeUpdated.Sex = Sex.Male;
        WriteDatabase.AddPersonToDatabase(personToBeUpdated);

        WriteDatabase.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{personToBeUpdated.Id}";
        var updates = new JsonPatchDocument<PartiallyUpdatePersonRequest>();
        updates.Remove(_ => _.Gender);
        updates.Remove(_ => _.Sex);
        updates.Remove(_ => _.DateOfBirth);
        var partiallyUpdateUserJson = JsonConvert.SerializeObject(updates);
        var payload = new StringContent(partiallyUpdateUserJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var httpResponse = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(1);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var actualUser = WriteDatabase.GetPersonFromDatabase(personToBeUpdated.Id);
        actualUser.Should().NotBeNull();
        actualUser.Gender.Should().BeNull();
        actualUser.Sex.Should().BeNull();
        actualUser.DateOfBirth.Should().BeNull();

        ObjectComparer.PublicInstancePropertiesEqual(personToBeUpdated, actualUser, "Sex", "Gender", "DateOfBirth", "Addresses", "Emails", "Phones", "Created", "Updated");

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
    public async Task Given_User_Exists___When_Add_Patch_Partial_Update_User___Then_200_OK_And_User_Partially_Updated()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        var alanTuring = new Person { FirstName = "Alan", LastName = "Turing" };
        WriteDatabase.AddPersonToDatabase(alanTuring);

        WriteDatabase.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{alanTuring.Id}";
        var johnDoe = new JsonPatchDocument<PartiallyUpdatePersonRequest>();
        johnDoe.Add(_ => _.Gender, Gender.Bigender);
        johnDoe.Add(_ => _.Sex, Sex.Male);
        johnDoe.Add(_ => _.DateOfBirth, "1912-06-23");
        var partiallyUpdateUserJson = JsonConvert.SerializeObject(johnDoe);
        var payload = new StringContent(partiallyUpdateUserJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var httpResponse = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(1);

        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
        var actualUser = WriteDatabase.GetPersonFromDatabase(alanTuring.Id);
        actualUser.Should().NotBeNull();
        actualUser.Gender.Should().Be(Gender.Bigender);
        actualUser.Sex.Should().Be(Sex.Male);
        actualUser.DateOfBirth.Should().Be(new DateTime(1912, 06, 23, 0, 0, 0, DateTimeKind.Utc));

        ObjectComparer.PublicInstancePropertiesEqual(alanTuring, actualUser, "Sex", "Gender", "DateOfBirth", "Addresses", "Emails", "Phones", "Created", "Updated");

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
    public async Task Given_User_Exists___When_Copy_Patch_Partial_Update_User___Then_200_OK_And_User_Partially_Updated()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        var personToBeUpdated = WriteDatabase.GenerateTestPerson();
        WriteDatabase.AddPersonToDatabase(personToBeUpdated);

        WriteDatabase.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{personToBeUpdated.Id}";
        var johnDoe = new JsonPatchDocument<PartiallyUpdatePersonRequest>();
        johnDoe.Copy(from => from.FirstName, to => to.LastName);
        var partiallyUpdatePersonJson = JsonConvert.SerializeObject(johnDoe);
        var payload = new StringContent(partiallyUpdatePersonJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var httpResponse = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(1);

        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var actualUser = WriteDatabase.GetPersonFromDatabase(personToBeUpdated.Id);
        actualUser.Should().NotBeNull();
        actualUser.LastName.Should().Be(personToBeUpdated.FirstName);

        ObjectComparer.PublicInstancePropertiesEqual(personToBeUpdated, actualUser, "LastName", "Addresses", "Emails", "Phones", "Created", "Updated");

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
    public async Task Given_Person_Does_Not_Exist___When_Patch_Partial_Update_Person___Then_201_Created_And_Person_Added()
    {
        // Arrange...
        WriteDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        var userId = Guid.NewGuid();
        var url = $"/api/person/{userId}";
        var patches = new JsonPatchDocument<PartiallyUpdatePersonRequest>();
        patches.Add(_ => _.FirstName, "John");
        patches.Add(_ => _.LastName, "Doe");
        var partiallyUpdateUserJson = JsonConvert.SerializeObject(patches);
        var payload = new StringContent(partiallyUpdateUserJson, System.Text.Encoding.UTF8, "application/json");

        // Act...
        var result = await HttpClient!.PatchAsync(url, payload);

        // Assert...
        WriteDatabase.NumberOfPeople().Should().Be(1);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var personResource = DeserializePersonResource(content);
        personResource.Should().NotBeNull();

        // - User
        personResource!.Data.Should().NotBeNull();
        ObjectComparer.PublicInstancePropertiesEqual(
            personResource.Data!, 
            new { Id = userId, FirstName = "John", LastName = "Doe" }, 
            "Gender", "Sex", "DateOfBirth").Should().BeTrue();

        // - Links
        personResource.Links.Should().NotBeNull();
        personResource.Links!.Count.Should().Be(2);
        LinksForPersonShouldBeCorrect(personResource.Links, personResource.Data!.Id);

        // - Embedded
        personResource.Embedded.Should().BeNull();

        // - Headers
        result.Headers.Location.Should().Be($"https://localhost/api/person/{userId}");

        // - Database
        var actualUser = WriteDatabase.GetPersonFromDatabase(userId);
        actualUser.Should().NotBeNull();
        ObjectComparer.PublicInstancePropertiesEqual(personResource.Data, actualUser, "Addresses", "Emails", "Phones", "Created", "Updated").Should().BeTrue();

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
        var userId = Guid.NewGuid();
        WriteDatabase.NumberOfPeople().Should().Be(0);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{userId}";
        var partiallyUpdateUserJson = "[" +
            "{" +
                "\"op\":\"remove\"," +
                "\"path\":\"/ThisDoesNotExist\"" +
            "}" +
        "]";
        var payload = new StringContent(partiallyUpdateUserJson, System.Text.Encoding.UTF8, "application/json");

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

        var personToBeUpdated = WriteDatabase.GenerateTestPerson();
        WriteDatabase.AddPersonToDatabase(personToBeUpdated);

        WriteDatabase.NumberOfPeople().Should().Be(1);

        await AuthenticateAsync(Username, Password);

        var url = $"/api/person/{personToBeUpdated.Id}";
        var partiallyUpdateUserJson = "[" +
            "{" +
                "\"op\":\"remove\"," +
                "\"path\":\"/ThisDoesNotExist\"" +
            "}" +
        "]";
        var payload = new StringContent(partiallyUpdateUserJson, System.Text.Encoding.UTF8, "application/json");

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
        var userId = Guid.NewGuid();
        WriteDatabase.NumberOfPeople().Should().Be(0);
        var url = $"/api/person/{userId}";
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
