﻿using System.Collections.Generic;

namespace Users.API.Write.Tests.v1.Methods;

public class OPTIONS
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
    public async Task Given_An_API_With_Unknown_Capabilities___When_An_Options_Call_Is_Made___Then_Allowed_HTTP_Methods_Are_Returned()
    {
        // Arrange...
        var expectedOptions = new List<string>() { "DELETE", "OPTIONS", "PATCH", "POST", "PUT"};

        // Act...
        const string url = $"/api/v1/users/";
        var message = new HttpRequestMessage(HttpMethod.Options, url);
        var httpResponse = await _client.SendAsync(message);

        // Assert...
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        httpResponse.Content.Headers.Allow.Count.Should().Be(5);
        httpResponse.Content.Headers.Allow.Should().BeEquivalentTo(expectedOptions);
        var content = await httpResponse.Content.ReadAsStringAsync();
        content.Length.Should().Be(0);
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
