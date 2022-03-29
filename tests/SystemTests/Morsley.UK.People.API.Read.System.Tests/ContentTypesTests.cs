namespace Morsley.UK.People.API.Read.System.Tests;

public class ContentTypesTests : ReadApplicationTestFixture<ReadProgram>
{
    //[Test]
    //[Category("Unhappy")]
    //public async Task Given_Only_Certain_Media_Types_Are_Supported___When_Making_A_Request_For_An_Unsupported_Type___Then_The_Status_Code_Should_Be_Not_Acceptable()
    //{
    //    // Arrange...
    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(0);

    //    var user = GenerateTestPerson();
    //    AddPersonToDatabase(user);

    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(1);

    //    var url = $"/api/person/{user.Id}";

    //    HttpClient!.DefaultRequestHeaders.Clear();
    //    HttpClient!.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

    //    await AuthenticateAsync(Username, Password);

    //    // Act...
    //    var httpResponse = await HttpClient!.GetAsync(url);

    //    // Assert...
    //    PeopleTestFixture.NumberOfPeopleInDatabase().Should().Be(1);

    //    httpResponse.IsSuccessStatusCode.Should().BeFalse();
    //    httpResponse.StatusCode.Should().Be(HttpStatusCode.NotAcceptable);
    //}
}
