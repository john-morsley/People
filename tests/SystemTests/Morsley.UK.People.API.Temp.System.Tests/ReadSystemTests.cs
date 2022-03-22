using FluentAssertions;
using NUnit.Framework;
using Morsley.UK.People.API.Test.Fixture;
using Serilog;
using Serilog.Events;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Morsley.UK.People.API.Temp.System.Tests
{
    public class ReadSystemTests : SecuredApplicationTestFixture<ReadProgram, SecurityProgram>
    {
        private const string ReadPersonUrl = "https://localhost:5001/api/person";
        private const string ReadPeopleUrl = "https://localhost:5001/api/people";

        public ReadSystemTests()
        {
        }

        [Test]
        public async Task Trying_To_Get_People_Without_Authenticating___Should_Result_In_401_Unauthorized()
        {
            // Act...
            var result = await HttpClient!.GetAsync(ReadPeopleUrl);

            // Assert...
            result.IsSuccessStatusCode.Should().Be(false);
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Trying_To_Get_A_Person___When_It_Does_Not_Exist___Should_Result_In_404_NotFound()
        {
            // Arrange...
            const string username = "johnmorsley";
            const string password = "P@$$w0rd!";
            await AuthenticateAsync(username, password);

            // Act...
            var url = $"{ReadPersonUrl}/{Guid.NewGuid()}";
            var result = await HttpClient!.GetAsync(url);

            // Assert...
            result.IsSuccessStatusCode.Should().Be(false);
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Trying_To_Get_People___When_None_Exist___Should_Result_In_204_NoContent()
        {
            // Arrange...
            const string username = "johnmorsley";
            const string password = "P@$$w0rd!";
            await AuthenticateAsync(username, password);

            // Act...
            var result = await HttpClient!.GetAsync(ReadPeopleUrl);

            // Assert...
            result.IsSuccessStatusCode.Should().Be(true);
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Test]
        public async Task Trying_To_Get_People_With_All_Parameters___When_None_Exist___Should_Result_In_204_NoContent()
        {
            // Arrange...
            const string username = "johnmorsley";
            const string password = "P@$$w0rd!";
            await AuthenticateAsync(username, password);

            // Act...
            var url = $"{ReadPeopleUrl}?pageNubmer=1&pageSize=10&filter=Sex:Male&fields=FirstName&search=John&sort=DateOfBirth:desc";
            var result = await HttpClient!.GetAsync(url);

            // Assert...
            result.IsSuccessStatusCode.Should().Be(true);
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        //[Test]
        //public async Task Trying_To_Get_Users___When_Some_Exist___Should_Result_In_200_OK()
        //{
        //    // Arrange...
        //    const string username = "johnmorsley";
        //    const string password = "P@$$w0rd!";
        //    await AuthenticateAsync(username, password);

        //    // Act...
        //    var url = $"{ReadUrl}?search=morsley";
        //    var result = await HttpClient.GetAsync(url);

        //    // Assert...
        //    result.IsSuccessStatusCode.Should().Be(true);
        //    result.StatusCode.Should().Be(HttpStatusCode.OK);
        //}
    }
}