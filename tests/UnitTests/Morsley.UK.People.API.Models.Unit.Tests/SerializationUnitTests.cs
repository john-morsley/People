namespace Morsley.UK.People.API.Models.Unit.Tests;

public class SerializationUnitTests
{

    [Test]
    [Category("Happy")]
    public void PagedList_Serializes_Successfully()
    {
        // Arrange...
        var people = new List<Morsley.UK.People.Domain.Models.Person>();
        var pagedList = new Morsley.UK.People.API.Contracts.Responses.PagedList<Morsley.UK.People.Domain.Models.Person>(people);

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PagedListJsonConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var serialized= JsonSerializer.Serialize(pagedList, options);

        // Assert...
        serialized.Should().NotBeNull();
        serialized.Should().Be("[]");
    }
}
