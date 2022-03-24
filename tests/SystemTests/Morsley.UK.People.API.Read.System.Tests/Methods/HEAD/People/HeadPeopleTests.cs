namespace Morsley.UK.People.API.Read.System.Tests.HEAD.People
{
    public class HeadPeopleTests : SecuredApplicationTestFixture<ReadProgram>
    {
        [Test]
        [Category("Happy")]
        public async Task Given_No_People_Exist___When_People_Are_Requested___Then_204_NoContent()
        {
            // Arrange...
            NumberOfPeopleInDatabase().Should().Be(0);

            const string url = "/api/people?pagenumber=1&pagesize=10";

            await AuthenticateAsync(Username, Password);

            // Act...
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var result = await HttpClient!.SendAsync(request);

            // Assert...
            NumberOfPeopleInDatabase().Should().Be(0);

            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            result.Content.Headers.ContentLength.Should().BeNull();

            var content = await result.Content.ReadAsStringAsync();
            content.Length.Should().Be(0);
        }

        [Test]
        [Category("Happy")]
        public async Task Given_People_Exists___When_People_Are_Requested___Then_200_OK_And_Content_Length_Returned()
        {
            // Arrange...
            NumberOfPeopleInDatabase().Should().Be(0);
            var user = GenerateTestPerson();
            AddPersonToDatabase(user);
            NumberOfPeopleInDatabase().Should().Be(1);

            const string url = "/api/people?PageNumber=1&PageSize=10";

            await AuthenticateAsync(Username, Password);

            var expectedContentLength = await DetermineExpectedContentLength(url);

            // Act...
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var result = await HttpClient!.SendAsync(request);

            // Assert...
            NumberOfPeopleInDatabase().Should().Be(1);

            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Headers.ContentLength.Should().Be(expectedContentLength);

            var content = await result.Content.ReadAsStringAsync();
            content.Length.Should().Be(0);
        }
    }
}
