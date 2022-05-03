using System;

namespace Morsley.UK.People.Application.Unit.Tests;

public class SerializationUnitTests
{

    [Test]
    [Category("Happy")]
    public void Empty_PagedList_Serializes_Successfully()
    {
        // Arrange...
        var people = new List<Morsley.UK.People.Domain.Models.Person>();
        var pagedList = PagedList<Person>.Create(people);

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PagedListOfPersonJsonConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var serialized= JsonSerializer.Serialize(pagedList, options);

        // Assert...
        serialized.Should().NotBeNull();
        var expectedJSON = "" +
        "{" +
            "\"CurrentPage\":0," +
            "\"PageSize\":0," +
            "\"TotalPages\":0," +
            "\"TotalCount\":0," +
            "\"HasPrevious\":false," +
            "\"HasNext\":false," +
            "\"Items\":" +
            "[" +
            "]" +
        "}";
        serialized.Should().Be(expectedJSON);
    }

    [Test]
    [Category("Happy")]
    public void PagedList_With_One_Item_Serializes_Successfully()
    {
        // Arrange...
        var people = new List<Morsley.UK.People.Domain.Models.Person>();
        var personId = Guid.NewGuid();
        var person = new Person(personId, "John", "Doe");
        people.Add(person);
        var pagedList = PagedList<Person>.Create(people);

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PagedListOfPersonJsonConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var serialized = JsonSerializer.Serialize(pagedList, options);

        // Assert...
        serialized.Should().NotBeNull();
        var expectedJSON = "" +
        "{" +
            "\"CurrentPage\":0," +
            "\"PageSize\":0," +
            "\"TotalPages\":0," +
            "\"TotalCount\":0," +
            "\"HasPrevious\":false," +
            "\"HasNext\":false," +
            "\"Items\":" +
            "[" +
                "{" +
                    "\"FirstName\":\"John\"," +
                    "\"LastName\":\"Doe\"," +
                    "\"Sex\":null," +
                    "\"Gender\":null," +
                    "\"DateOfBirth\":null," +
                    "\"Emails\":[]," +
                    "\"Phones\":[]," +
                    "\"Addresses\":[]," +
                    "\"Created\":\"0001-01-01T00:00:00\"," +
                    "\"Updated\":null," +
                    $"\"Id\":\"{personId}\"" +
                "}" +
            "]" +
        "}";

        serialized.Should().Be(expectedJSON);
    }

}
