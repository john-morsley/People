namespace Morsley.UK.People.API.Contracts.Tests;

public class PersonResponseConverterTests
{

    /*

    { 
        "Id":"53e48a17-5a36-4820-9fbc-0beeef63164e",
        "FirstName":"FirstName___2bc71b33-24ae-4ba5-84b7-033686432247",
        "LastName":"LastName___c00c3395-fd91-420f-966e-d84b7ccf3bea",
        "Sex":null,
        "Gender":"Transgender",
        "DateOfBirth":"1941-01-16",
        "_links":[
            {"hypertextReference":"http://localhost/api/v1/users/53e48a17-5a36-4820-9fbc-0beeef63164e","relationship":"self","method":"GET"},
            {"hypertextReference":"http://localhost/api/v1/users/53e48a17-5a36-4820-9fbc-0beeef63164e","relationship":"self","method":"DELETE"}
        ]
    }
    
    */

    /*
        {
            "_embedded":[
                {
                    "Id":"",
                    "FirstName":"",
                    "LastName":"",
                    "_links":[
                        {"hypertextReference":"http://localhost/api/v1/users/{personId}","relationship":"self","method":"GET"},
                        {"hypertextReference":"http://localhost/api/v1/users/{userID}","relationship":"self","method":"DELETE"}
                    ]
                }
            ],
            "_links":[
                {"hypertextReference":"http://localhost/api/v1/users?pageNumber=1&pageSize=10&sort=LastName%3Aasc,FirstName%3Aasc","relationship":"self","method":"GET"}
            ]
        }
    */

    [Test]
    [Category("Happy")]
    public void Given_Valid_But_Minimal_JSON___When_Deserialized_With_Converter___Then_Should_Produce_Person_Re_Object()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        var json = 
            "{" +
            $"\"Id\":\"{personId}\"," +
            "\"FirstName\":\"John\"," +
            "\"LastName\":\"Doe\"" +
            "}";

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var personResource = JsonSerializer.Deserialize<PersonResource>(json, options);

        // Assert...
        personResource.Should().NotBeNull();
        personResource.Data.Should().NotBeNull();
        personResource.Data.Id.Should().Be(personId);
        personResource.Data.FirstName.Should().Be("John");
        personResource.Data.LastName.Should().Be("Doe");
        personResource.Data.DateOfBirth.Should().BeNull();
        personResource.Data.Gender.Should().BeNull();
        personResource.Data.Sex.Should().BeNull();
        personResource.Links.Should().BeNull();
        personResource.Embedded.Should().BeNull();
    }

    [Test]
    [Category("Happy")]
    public void Given_Valid_And_More_Complete_JSON___When_Deserialized_With_Converter___Then_Should_Produce_Person_Data_Object()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        var dob = "2001-02-03";
        var json = 
            "{" +
            $"\"Id\":\"{personId}\"," +
            "\"FirstName\":\"John\"," +
            "\"LastName\":\"Doe\"," +
            $"\"DateOfBirth\":\"{dob}\"," +
            "\"Sex\":\"Male\"," +
            "\"Gender\":\"Cisgender\"" +
            "}";

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var personResource = JsonSerializer.Deserialize<PersonResource>(json, options);

        // Assert...
        personResource.Should().NotBeNull();
        personResource.Data.Should().NotBeNull();
        personResource.Data.Id.Should().Be(personId);
        personResource.Data.FirstName.Should().Be("John");
        personResource.Data.LastName.Should().Be("Doe");
        personResource.Data.DateOfBirth.Should().Be(dob);
        personResource.Data.Gender.Should().Be(Gender.Cisgender.ToString());
        personResource.Data.Sex.Should().Be(Sex.Male.ToString());
        personResource.Links.Should().BeNull();
        personResource.Embedded.Should().BeNull();
    }

    [Test]
    [Category("Happy")]
    public void Given_Valid_And_More_Complete_JSON_But_With_Empty_Values___When_Deserialized_With_Converter___Then_Should_Produce_Person_Data_Object()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        var json =
            "{" +
            $"\"Id\":\"{personId}\"," +
            "\"FirstName\":\"John\"," +
            "\"LastName\":\"Doe\"," +
            "\"DateOfBirth\":\"\"," +
            "\"Sex\":\"\"," +
            "\"Gender\":\"\"" +
            "}";

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var personResource = JsonSerializer.Deserialize<PersonResource>(json, options);

        // Assert...
        personResource.Should().NotBeNull();
        personResource?.Data.Should().NotBeNull();
        personResource?.Data.Id.Should().Be(personId);
        personResource?.Data.FirstName.Should().Be("John");
        personResource?.Data.LastName.Should().Be("Doe");
        personResource?.Data.DateOfBirth.Should().BeNull();
        personResource?.Data.Gender.Should().BeNull();
        personResource?.Data.Sex.Should().BeNull();
        personResource?.Links.Should().BeNull();
        personResource?.Embedded.Should().BeNull();
    }

    [Test]
    [Category("Happy")]
    public void Given_Valid_JSON_With_Links___When_Deserialized_With_Converter___Then_Should_Produce_Person_Data_Object()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        var json = 
            "{" +
            $"\"Id\":\"{personId}\"," +
            "\"FirstName\":\"John\"," +
            "\"LastName\":\"Doe\"," +
            "\"_links\":[" +
            $"{{\"hypertextReference\":\"http://localhost/api/person/{personId}\",\"relationship\":\"self\",\"method\":\"GET\"}}," +
            $"{{\"hypertextReference\":\"http://localhost/api/person/{personId}\",\"relationship\":\"self\",\"method\":\"DELETE\"}}" +
            "]" +
            "}";

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var personResource = JsonSerializer.Deserialize<PersonResource>(json, options);

        // Assert...
        personResource.Should().NotBeNull();
        personResource?.Data.Should().NotBeNull();
        personResource?.Data.Id.Should().Be(personId);
        personResource?.Data.FirstName.Should().Be("John");
        personResource?.Data.LastName.Should().Be("Doe");
        personResource?.Data.DateOfBirth.Should().BeNull();
        personResource?.Data.Gender.Should().BeNull();
        personResource?.Data.Sex.Should().BeNull();
        personResource?.Links.Should().NotBeNull();
        personResource?.Links.Count.Should().Be(2);
        var getLink = personResource?.Links.SingleOrDefault(_ => _.Method == "GET" && _.Relationship == "self");
        getLink.Should().NotBeNull();
        getLink?.HypertextReference.Should().Be($"http://localhost/api/person/{personId}");
        var deleteLink = personResource?.Links.SingleOrDefault(_ => _.Method == "DELETE" && _.Relationship == "self");
        deleteLink?.HypertextReference.Should().Be($"http://localhost/api/person/{personId}");
        deleteLink.Should().NotBeNull();

        personResource?.Embedded.Should().BeNull();
    }

    [Test]
    [Category("Happy")]
    //[TestCase()]
    public void Given_Valid_JSON_With_Embedded___When_Deserialized_With_Converter___Then_Should_Produce_Person_Data_Object()
    {
        // Arrange...
        var personId = Guid.NewGuid();
        var json =
            "{" +
            "\"_embedded\":[" +
            "{" +
            $"\"Id\":\"{personId}\"," +
            "\"FirstName\":\"John\"," +
            "\"LastName\":\"Doe\"," +
            "\"_links\":[" +
            $"{{\"hypertextReference\":\"http://localhost/api/person/{personId}\",\"relationship\":\"self\",\"method\":\"GET\"}}," +
            $"{{\"hypertextReference\":\"http://localhost/api/person/{personId}\",\"relationship\":\"self\",\"method\":\"DELETE\"}}" +
            "]" +
            "}" +
            "]" +
            "}";

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var personResource = JsonSerializer.Deserialize<PersonResource>(json, options);

        // Assert...
        personResource.Should().NotBeNull();
        personResource?.Data.Should().BeNull();
        personResource?.Links.Should().BeNull();
        
        personResource?.Embedded.Should().NotBeNull();
        personResource?.Embedded.Count.Should().Be(1);

        var embeddedPersonData = personResource?.Embedded.FirstOrDefault();
        embeddedPersonData.Should().NotBeNull();
        embeddedPersonData?.Links.Count.Should().Be(2);
        embeddedPersonData?.Data.Id.Should().Be(personId);
        embeddedPersonData?.Data.FirstName.Should().Be("John");
        embeddedPersonData?.Data.LastName.Should().Be("Doe");
        embeddedPersonData?.Data.DateOfBirth.Should().BeNull();
        embeddedPersonData?.Data.Gender.Should().BeNull();
        embeddedPersonData?.Data.Sex.Should().BeNull();

        var getLink = embeddedPersonData?.Links.SingleOrDefault(_ => _.Method == "GET" && _.Relationship == "self");
        getLink.Should().NotBeNull();
        getLink?.HypertextReference.Should().Be($"http://localhost/api/person/{personId}");
        
        var deleteLink = embeddedPersonData?.Links.SingleOrDefault(_ => _.Method == "DELETE" && _.Relationship == "self");
        deleteLink?.HypertextReference.Should().Be($"http://localhost/api/person/{personId}");
        deleteLink.Should().NotBeNull();
    }

    [Test]
    [Category("Unhappy")]
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("InvalidValue")]
    public void Given_JSON_With_Invalid_Person_ID___When_Deserialized_With_Converter___Then_Should_Throw_InvalidOperationException(string invalidPersonId)
    {
        // Arrange...
        var json =
            "{" +
            $"\"Id\":\"{invalidPersonId}\"," +
            "\"FirstName\":\"John\"," +
            "\"LastName\":\"Doe\"," +
            "}";

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PersonResource>(json, options));

        // Assert...
        exception.Should().NotBeNull();
        exception?.Message.StartsWith("The JSON value could not be converted to Morsley.UK.People.API.Contracts.Shared.PersonResource.").Should().BeTrue();
        exception?.InnerException.Should().NotBeNull();
        exception?.InnerException?.Message.Should().Be("The JSON value is not in a supported Guid format.");



        /*
         * The JSON value could not be converted to PersonResource.
         * 12345678901234567890123456789012345678901234567890123456789012345678901234
         *          1         2         3         4         5         6         7
         *
         *
         * The JSON value is not in a supported Guid format.
         */

    }

    [Test]
    [Category("Unhappy")]
    [TestCase(" ")]
    [TestCase("InvalidValue")]
    [TestCase("01/02/2003")]
    [TestCase("01-02-2003")]
    public void Given_JSON_With_Invalid_DateOfBirth___When_Deserialized_With_Converter___Then_Should_Throw_InvalidOperationException(string invalidDob)
    {
        // Arrange...
        var personId = Guid.NewGuid();
        var json =
            "{" +
            $"\"Id\":\"{personId}\"," +
            "\"FirstName\":\"John\"," +
            "\"LastName\":\"Doe\"," +
            $"\"DateOfBirth\":\"{invalidDob}\"" +
            "}";

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var exception = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<PersonResource>(json, options));

        // Assert...
        exception?.Message.Should().Be($"DateOfBirth is not valid: Expected format is 'YYYY-MM-DD', actual value: '{invalidDob}'");
    }

    [Test]
    [Category("Unhappy")]
    [TestCase(" ")]
    [TestCase("InvalidValue")]
    public void Given_JSON_With_Invalid_Sex___When_Deserialized_With_Converter___Then_Should_Throw_InvalidOperationException(string sex)
    {
        // Arrange...
        var personId = Guid.NewGuid();
        var json =
            "{" +
            $"\"Id\":\"{personId}\"," +
            "\"FirstName\":\"John\"," +
            "\"LastName\":\"Doe\"," +
            $"\"Sex\":\"{sex}\"" +
            "}";

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var exception = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<PersonResource>(json, options));

        // Assert...
        exception?.Message.Should().Be($"Sex is not valid: Actual value: '{sex}'");
    }

    [Test]
    [Category("Unhappy")]
    [TestCase(" ")]
    [TestCase("InvalidValue")]
    public void Given_JSON_With_Invalid_Gender___When_Deserialized_With_Converter___Then_Should_Throw_InvalidOperationException(string gender)
    {
        // Arrange...
        var personId = Guid.NewGuid();
        var json =
            "{" +
            $"\"Id\":\"{personId}\"," +
            "\"FirstName\":\"John\"," +
            "\"LastName\":\"Doe\"," +
            $"\"Gender\":\"{gender}\"" +
            "}";

        // Act...
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new PersonResourceConverter(),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        var exception = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<PersonResource>(json, options));

        // Assert...
        exception?.Message.Should().Be($"Gender is not valid: Actual value: '{gender}'");
    }
}