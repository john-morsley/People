using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Users.API.Write.Tests.v1
{
    public class Swagger
    {
        private TestServer _server;
        private HttpClient _client;
        
        [SetUp]
        public void Setup()
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json");
                })
                .UseEnvironment("Development")
                .UseStartup<StartUp>();
            _server = new TestServer(webHostBuilder);
            _client = _server.CreateClient();
        }

        [Test]
        public async Task GetDefaultSwaggerWithIncompleteUrl()
        {
            // Arrange...
            const string url = "";

            // Act...
            var httpResponse = await _client.GetAsync(url);
            
            // Assert...
            httpResponse.IsSuccessStatusCode.Should().BeFalse();
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Moved);
            httpResponse.Headers.Location.Should().Be("index.html");
        }

        [Test]
        public async Task GetDefaultSwaggerWithCompleteUrl()
        {
            // Arrange...
            const string url = "/index.html";

            // Act...
            var httpResponse = await _client.GetAsync(url);

            // Assert...
            httpResponse.IsSuccessStatusCode.Should().BeTrue();
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await httpResponse.Content.ReadAsStringAsync();
            response.Length.Should().BeGreaterThan(0);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);
            var title = htmlDocument.DocumentNode.SelectSingleNode("//title").InnerText;
            title.Should().Be("Swagger UI");
        }

        [Test]
        public async Task The_Endpoint_For_The_Swagger_JSON_Should_Yield_OpenAPI_JSON()
        {
            // Arrange...
            const string url = "/swagger/v1/swagger.json";

            // Act...
            var httpResponse = await _client.GetAsync(url);

            // Assert...
            httpResponse.IsSuccessStatusCode.Should().BeTrue();
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await httpResponse.Content.ReadAsStringAsync();
            response.Length.Should().BeGreaterThan(0);
            var swagger = JObject.Parse(response);
            swagger.Count.Should().Be(4);
        }


        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}