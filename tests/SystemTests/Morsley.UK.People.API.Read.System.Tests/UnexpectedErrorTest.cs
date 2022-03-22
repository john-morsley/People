namespace Morsley.UK.People.API.Read.System.Tests;

public class ExceptionHandler : ApplicationTestFixture<ReadProgram>
{
    //private TestServer _server;
    //private HttpClient HttpClient;

    //[SetUp]
    //public void Setup()
    //{
    //    var webHostBuilder = new WebHostBuilder()
    //        .UseEnvironment("Production")
    //        .UseStartup<StartUp>();
    //    _server = new TestServer(webHostBuilder);
    //    HttpClient = _server.CreateClient();
    //}

    /*
     * This was the only way I could think of to get the API to throw an internal Server Error.
     */
    //[Test]
    //[Category("Unhappy")]
    //public async Task Given_The_API_Is_In_Production_Mode___When_It_Is_Not_Passed_Vital_Configuration___Then_An_Error_Code_And_Error_Message_Is_Returned()
    //{
    //    // Arrange...
    //    var userId = Guid.NewGuid();

    //    var url = $"/api/person/{userId}";

    //    // Act...
    //    var httpResponse = await HttpClient.GetAsync(url);

    //    // Assert...
    //    httpResponse.IsSuccessStatusCode.Should().BeFalse();
    //    httpResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    //    httpResponse.ReasonPhrase.Should().Be("Oops, I didn't expect that to happen! :-(");
    //    var response = await httpResponse.Content.ReadAsStringAsync();
    //    response.Length.Should().Be(0);
    //}

    //[TearDown]
    //public void TearDown()
    //{
    //    HttpClient.Dispose();
    //    _server.Dispose();
    //}
}