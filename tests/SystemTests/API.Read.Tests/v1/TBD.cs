using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Users.API.Read.Tests.v1
{
    public class TBD
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



        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}