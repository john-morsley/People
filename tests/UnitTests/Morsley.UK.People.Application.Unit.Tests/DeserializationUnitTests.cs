using System;

namespace Morsley.UK.People.Application.Unit.Tests;

public class DeserializationUnitTests
{
    [Test]
    [Category("Happy")]
    public void Empty_PagedList_Deserializes_Successfully()
    {
        // Arrange...
        var actualJSON = "" +
        "{" +
            "\"CurrentPage\":0," +
            "\"PageSize\":0," +
            "\"TotalPages\":0," +
            "\"TotalCount\":0," +
            "\"HasPrevious\":false," +
            "\"HasNext\":false," +
            "\"Items\":[]" +
        "}";

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
        var deserialized = JsonSerializer.Deserialize<PagedList<Person>>(actualJSON, options);

        // Assert...
        var people = new List<Person>();
        var pagedList = PagedList<Person>.Create(people);
        deserialized.Should().NotBeNull();
        deserialized.Should().BeEquivalentTo(pagedList);
    }

    [Test]
    [Category("Happy")]
    public void PagedList_With_One_Item_Deserializes_Successfully()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        var actualJSON = "" +
        "{" +
            "\"CurrentPage\":1," +
            "\"PageSize\":1," +
            "\"TotalPages\":1," +
            "\"TotalCount\":1," +
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
        var deserialized = JsonSerializer.Deserialize<PagedList<Person>>(actualJSON, options);

        // Assert...
        var people = new List<Person>();
        var person = new Person(personId, "John", "Doe");
        people.Add(person);
        var pagedList = PagedList<Person>.Create(people);
        deserialized.Should().NotBeNull();
        deserialized.Should().BeEquivalentTo(pagedList);
    }

}
