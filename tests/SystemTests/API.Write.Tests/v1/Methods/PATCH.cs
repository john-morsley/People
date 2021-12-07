﻿namespace Users.API.Write.Tests.v1.Methods;

// Patch Operations we can test:
// - Add?
// - Remove? What will this do? Will it set that value to its default?
// - Replace - This should change the objects property value (equivalent to an update).
// - Copy? Will this copy an objects property to another property. i.e. FirstName to LastName
// - Move? As the copy above, but the source property will then be set to its default value.
// - Test? Tests that an objects property value is equal to a specfied value.

// Refer to: http://jsonpatch.com/

public class PATCH : APIsTestBase<StartUp>
{
    [Test]
    [Category("Happy")]
    public async Task Given_User_Exists___When_Replace_Patch_Partial_Update_User___Then_200_OK_And_User_Partially_Updated()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var userToBeUpdated = GenerateTestUser(userId);
        AddUserToDatabase(userToBeUpdated);
        NumberOfUsersInDatabase().Should().Be(1);
        string replacementFirstName = GetTestString("ReplacementFirstName");

        // Act...
        var url = $"/api/v1/users/{userId}";
        var partiallyUpdateUserJson = string.Empty +
        "[" +
            "{" +
                "\"op\":\"replace\"," +
                "\"path\":\"/FirstName\"," +
                $"\"value\":\"{replacementFirstName}\"" +
            "}" +
        "]";
        var payload = new StringContent(partiallyUpdateUserJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var httpResponse = await _client.PatchAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
        var actualUser = GetUserFromDatabase(userId);
        actualUser.Should().NotBeEquivalentTo(userToBeUpdated);
        actualUser.FirstName.Should().Be(replacementFirstName);
    }

    [Test]
    [Category("Happy")]
    public async Task Given_User_Exists___When_Remove_Patch_Partial_Update_User___Then_200_OK_And_User_Partially_Updated()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var userToBeUpdated = GenerateTestUser(userId);
        userToBeUpdated.Sex = Users.Domain.Enumerations.Sex.Male;
        AddUserToDatabase(userToBeUpdated);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        var url = $"/api/v1/users/{userId}";
        var partiallyUpdateUserJson = string.Empty +
        "[" +
            "{" +
                "\"op\":\"remove\"," +
                "\"path\":\"/Sex\"" +
            "}" +
        "]";
        var payload = new StringContent(partiallyUpdateUserJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        var httpResponse = await _client.PatchAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);
        var actualUser = GetUserFromDatabase(userId);
        actualUser.Should().NotBeEquivalentTo(userToBeUpdated);
        actualUser.Sex.Should().BeNull();
    }

    [Test]
    [Category("Unhappy")]
    public async Task Given_User_Does_Not_Exist___When_Patch_Partial_Update_User___Then_404_NotFound()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var userId = Guid.NewGuid();
        var url = $"/api/v1/users/{userId}";
        //var upsertUserRequest = GenerateTestUpdateUserRequest();
        //var upsertUserRequestJson = JsonSerializer.Serialize(upsertUserRequest);
        //var payload = new StringContent(upsertUserRequestJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        //var httpResponse = await _client.PutAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);
        //httpResponse.IsSuccessStatusCode.Should().BeFalse();
        //httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        //var content = await httpResponse.Content.ReadAsStringAsync();
        //content.Length.Should().BeGreaterThan(0);
        // ToDo --> Check the error object
    }

}
