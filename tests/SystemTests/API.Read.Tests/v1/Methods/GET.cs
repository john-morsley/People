using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Users.API.Read.Tests.v1.Methods
{
    public class GET
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
        public async Task GetUser()
        {
            // Arrange...
            var userId = Guid.NewGuid();
            var url = $"/api/v1/users/{userId}";
            
            // Act...
            var httpResponse = await _client.GetAsync(url);
            
            // Assert...
            //httpResponse.IsSuccessStatusCode.Should().BeTrue();
            //httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            //var response = await httpResponse.Content.ReadAsStringAsync();
            //response.Length.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task GetUsers()
        {
            // Arrange...
            var userId = Guid.NewGuid();
            var url = $"/api/v1/users/";
            var getUsersRequest = new Users.API.Models.Request.v1.GetPageOfUsersRequest();
            
            // Act...
            var httpResponse = await _client.GetAsync(url);
            
            // Assert...
            httpResponse.IsSuccessStatusCode.Should().BeTrue();
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await httpResponse.Content.ReadAsStringAsync();
            response.Length.Should().BeGreaterThan(0);
            var users = JsonConvert.DeserializeObject<IEnumerable<Users.API.Models.Response.v1.UserResponse>>(response);
            users.Count().Should().Be(1);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}