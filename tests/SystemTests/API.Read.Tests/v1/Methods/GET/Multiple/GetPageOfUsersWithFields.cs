namespace Users.API.Read.Tests.v1.Methods.GET.Multiple;

public class GetPageOfUsersWithFields : APIsTestBase<StartUp>
{    
    [Test]
    [Category("Happy")]
    public async Task Given_One_User_Exists___When_A_Page_Of_Users_Is_Requested_With_Fields___Then_200_OK_And_Users_Should_Be_Shaped()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var user = GenerateTestUser();
        AddUserToDatabase(user);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        var url = $"/api/v1/users?fields=id,lastname";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);

        var userData = DeserializeUserData(response);
        userData.Should().NotBeNull();
        //pageOfUsers.Count().Should().Be(1);

        //var actual = pageOfUsers.First();
        //actual.Should().NotBeNull();
        //actual.Id.Should().Be(user.Id);
        //actual.FirstName.Should().BeNull();
        //actual.LastName.Should().Be(user.LastName);
        //actual.Sex.Should().BeNull();
        //actual.Gender.Should().BeNull();

        //IEnumerable<string> values;
        //httpResponse.Headers.TryGetValues("X-Pagination", out values);
        //values.Should().NotBeNull();
        //values.Count().Should().Be(1);

        //var pagination = JsonSerializer.Deserialize<Users.API.Models.Shared.Pagination>(values.FirstOrDefault());
        //pagination.PreviousPageLink.Should().BeNull();
        //pagination.NextPageLink.Should().BeNull();
        //pagination.CurrentPage.Should().Be(1);
        //pagination.TotalPages.Should().Be(1);
        //pagination.TotalCount.Should().Be(1);
        //pagination.PageSize.Should().Be(10);
    }

    [Test]
    [Category("Unhappy")]
    public async Task When_A_Page_Of_Users_Is_Requested_With_Invalid_Fields___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var url = $"/api/v1/users?fields=fielddoesnotexist";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);

        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);

        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(response);
        problemDetails.Should().NotBeNull();
        problemDetails.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        problemDetails.Title.Should().Be("Validation error(s) occurred!");
        problemDetails.Detail.Should().Be("See the errors field for details.");
        problemDetails.Instance.Should().Be($"/api/v1/users");
        problemDetails.Extensions.Should().NotBeNull();
        var traceId = problemDetails.Extensions.Where(_ => _.Key == "traceId").FirstOrDefault();
        traceId.Should().NotBeNull();
        problemDetails.Errors.Count().Should().Be(1);
        var error = problemDetails.Errors.First();
        error.Key.Should().Be("Fields");
        var value = error.Value.First();
        value.Should().Be("The fields value is invalid. e.g. fields=id,lastname");
    }
}
