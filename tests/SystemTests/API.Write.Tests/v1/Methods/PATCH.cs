namespace Users.API.Write.Tests.v1.Methods;

public class PATCH : APIsTestBase<StartUp>
{
    [Test]
    public async Task Given_User_Exists___When_Patch_Partial_Update_User___Then_200_OK_And_User_Partially_Updated()
    {
        // Arrange...
        var userId = Guid.NewGuid();
        var userToBeUpdated = GenerateTestUser(userId);
        AddUserToDatabase(userToBeUpdated);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        var url = $"/api/v1/users/{userId}";
        //var updateUserRequest = GenerateTestUpdateUserRequest();
        //var upsertUserRequestJson = JsonSerializer.Serialize(updateUserRequest);
        // var payload = new StringContent(upsertUserRequestJson, System.Text.Encoding.UTF8, API_MEDIA_TYPE);
        //var httpResponse = await _client.PutAsync(url, payload);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        //httpResponse.IsSuccessStatusCode.Should().BeTrue();
        //httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        //var content = await httpResponse.Content.ReadAsStringAsync();
        //content.Length.Should().BeGreaterThan(0);
        //var actualUser = GetUserFromDatabase(userId);
        //actualUser.Should().NotBeEquivalentTo(userToBeUpdated);
        //actualUser.Should().BeEquivalentTo(updateUserRequest);
    }

    [Test]
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
