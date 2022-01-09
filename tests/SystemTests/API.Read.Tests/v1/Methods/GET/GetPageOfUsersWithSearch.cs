namespace Users.API.Read.Tests.v1.Methods;

public class GetPageOfUsersWithSearch : APIsTestBase<StartUp>
{
    [Test]
    [Category("Happy")]
    public async Task Given_Users_Exist___When_A_Page_Of_Users_Is_Requested_With_Search_Criteria___Then_200_OK_And_Users_Should_Be_Limited()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var johnDoe = new Users.Domain.Models.User() { FirstName = "John", LastName = "Doe" };
        AddUserToDatabase(johnDoe);
        var janeDoe = new Users.Domain.Models.User() { FirstName = "Jane", LastName = "Doe" };
        AddUserToDatabase(janeDoe);
        var saadMaan = new Users.Domain.Models.User() { FirstName = "Saad", LastName = "Man" };
        AddUserToDatabase(saadMaan);
        var whetFaartz = new Users.Domain.Models.User() { FirstName = "Whet", LastName = "Faartz" };
        AddUserToDatabase(whetFaartz);
        var fredBloggs = new Users.Domain.Models.User() { FirstName = "Fred", LastName = "Bloggs" };
        AddUserToDatabase(fredBloggs);
        var fayeBloggs = new Users.Domain.Models.User() { FirstName = "Faye", LastName = "Bloggs" };
        AddUserToDatabase(fayeBloggs);
        NumberOfUsersInDatabase().Should().Be(6);

        // Act...
        var url = $"/api/v1/users?search=aa";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(6);

        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);

        var pageOfUsers = DeserializeListOfUserResponses(response);
        pageOfUsers.Should().NotBeNull();
        pageOfUsers.Count().Should().Be(2);

        var firstUser = pageOfUsers.Skip(0).Take(1).Single();
        var secondUser = pageOfUsers.Skip(1).Take(1).Single();

        firstUser.FirstName.Should().Be(whetFaartz.FirstName);
        firstUser.LastName.Should().Be(whetFaartz.LastName);
        secondUser.FirstName.Should().Be(saadMaan.FirstName);
        secondUser.LastName.Should().Be(saadMaan.LastName);

        IEnumerable<string> values;
        httpResponse.Headers.TryGetValues("X-Pagination", out values);
        values.Should().NotBeNull();
        values.Count().Should().Be(1);

        var pagination = JsonSerializer.Deserialize<Users.API.Models.Shared.Pagination>(values.FirstOrDefault());
        pagination.PreviousPageLink.Should().BeNull();
        pagination.NextPageLink.Should().BeNull();
        pagination.CurrentPage.Should().Be(1);
        pagination.TotalPages.Should().Be(1);
        pagination.TotalCount.Should().Be(2);
        pagination.PageSize.Should().Be(10);
    }
}
