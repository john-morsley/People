//namespace Users.API.Write.Tests.v1;

//public class SwaggerTests : ApplicationTestFixture<WriteProgram>
//{       
//    [Test]
//    [Category("Unhappy")]
//    public async Task GetDefaultSwaggerWithIncompleteUrl()
//    {
//        // Arrange...
//        const string url = "";

//        // Act...
//        var httpResponse = await HttpClient.GetAsync(url);
            
//        // Assert...
//        httpResponse.IsSuccessStatusCode.Should().BeFalse();
//        httpResponse.StatusCode.Should().Be(HttpStatusCode.Moved);
//        httpResponse.Headers.Location.Should().Be("index.html");
//    }

//    [Test]
//    [Category("Happy")]
//    public async Task GetDefaultSwaggerWithCompleteUrl()
//    {
//        // Arrange...
//        const string url = "/index.html";

//        // Act...
//        var httpResponse = await HttpClient.GetAsync(url);

//        // Assert...
//        httpResponse.IsSuccessStatusCode.Should().BeTrue();
//        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
//        var response = await httpResponse.Content.ReadAsStringAsync();
//        response.Length.Should().BeGreaterThan(0);
//        var htmlDocument = new HtmlDocument();
//        htmlDocument.LoadHtml(response);
//        var title = htmlDocument.DocumentNode.SelectSingleNode("//title").InnerText;
//        title.Should().Be("Swagger UI");
//    }

//    [Test]
//    [Category("Happy")]
//    public async Task The_Endpoint_For_The_Swagger_JSON_Should_Yield_OpenAPI_JSON()
//    {
//        // Arrange...
//        const string url = "/swagger/v1/swagger.json";

//        // Act...
//        var httpResponse = await HttpClient.GetAsync(url);

//        // Assert...
//        httpResponse.IsSuccessStatusCode.Should().BeTrue();
//        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
//        var response = await httpResponse.Content.ReadAsStringAsync();
//        response.Length.Should().BeGreaterThan(0);
//        var swagger = JObject.Parse(response);
//        swagger.Count.Should().Be(4);
//    }
//}
