namespace Morsley.UK.People.API.Read.System.Tests.Methods
{
    public class HEAD : SecuredApplicationTestFixture<ReadProgram, SecurityProgram>
    {
        [Test]
        [Category("Happy")]
        public async Task Given_The_Person_Does_Not_Exist_In_The_Database___When_That_Person_Is_Requested___Then_204_NoContent()
        {
            // Arrange...
            var userId = Guid.NewGuid();
            var url = $"/api/person/{userId}";

            await AuthenticateAsync(Username, Password);

            // Act...
            var result = await HttpClient!.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

            // Assert...
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            result.Content.Headers.ContentLength.Should().BeNull();

            var content = await result.Content.ReadAsStringAsync();        
            content.Length.Should().Be(0);
        }

        [Test]
        [Category("Happy")]
        public async Task Given_The_Person_Exists___When_That_Person_Is_Requested___Then_200_OK_And_Content_Length_Returned()
        {
            // Arrange...
            NumberOfPeopleInDatabase().Should().Be(0);
            var expected = GenerateTestPerson();
            AddPersonToDatabase(expected);
            NumberOfPeopleInDatabase().Should().Be(1);

            var url = $"/api/person/{expected.Id}";

            await AuthenticateAsync(Username, Password);

            var expectedContentLength = await GetExpectedContentLength(url);

            // Act...
            var result = await HttpClient!.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

            // Assert...
            NumberOfPeopleInDatabase().Should().Be(1);

            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Headers.ContentLength.Should().Be(expectedContentLength);

            var content = await result.Content.ReadAsStringAsync();
            content.Length.Should().Be(0);
        }

        [Test]
        [Category("Happy")]
        public async Task Given_No_People_Exist___When_People_Are_Requested___Then_204_NoContent()
        {
            // Arrange...
            NumberOfPeopleInDatabase().Should().Be(0);

            const string url = "/api/people?PageNumber=1&PageSize=10";

            await AuthenticateAsync(Username, Password);

            // Act...
            var result = await HttpClient!.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

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

            var expectedContentLength = await GetExpectedContentLength(url);

            // Act...
            var result = await HttpClient!.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

            // Assert...
            NumberOfPeopleInDatabase().Should().Be(1);

            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.Headers.ContentLength.Should().Be(expectedContentLength);

            var content = await result.Content.ReadAsStringAsync();
            content.Length.Should().Be(0);
        }

        private async Task<long> GetExpectedContentLength(string url)
        {
            var result = await HttpClient!.GetAsync(url);
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await result.Content.ReadAsStringAsync();
            return content.Length;
        }
    }
}
