namespace Morsley.UK.People.API.Models.Unit.Tests;

public class DeserializationUnitTests
{
    [Test]
    [Category("Happy")]
    public void PagedList_Deserializes_Successfully()
    {
        // Arrange...
        var json = "[]";

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
        var deserialized = JsonSerializer.Deserialize<Morsley.UK.People.API.Contracts.Responses.PagedList<Morsley.UK.People.Domain.Models.Person>>(json, options);

        // Assert...
        var people = new List<Morsley.UK.People.Domain.Models.Person>();
        var pagedList = new Morsley.UK.People.API.Contracts.Responses.PagedList<Morsley.UK.People.Domain.Models.Person>(people);
        deserialized.Should().NotBeNull();
        deserialized.Should().BeEquivalentTo(pagedList);
    }
}
