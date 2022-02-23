using System.Dynamic;
using System.Linq;
using System.Text.Json.Serialization;
using Users.API.Models.Response.v1;

namespace API.Models.Shared.Tests
{
    public class UserResponseConverterTests
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
                            {"hypertextReference":"http://localhost/api/v1/users/{userId}","relationship":"self","method":"GET"},
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
        public void Given_Valid_But_Minimal_JSON___When_Deserialized_With_Converter___Then_Should_Produce_User_Re_Object()
        {
            // Arrange...
            var userId = Guid.NewGuid();
            var json = 
            "{" +
                $"\"Id\":\"{userId}\"," +
                "\"FirstName\":\"John\"," +
                "\"LastName\":\"Doe\"" +
            "}";

            // Act...
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new Users.API.Models.Shared.UserResourceConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var userResource = JsonSerializer.Deserialize<Users.API.Models.Shared.UserResource>(json, options);

            // Assert...
            userResource.Should().NotBeNull();
            userResource?.Data.Should().NotBeNull();
            userResource?.Data.Id.Should().Be(userId);
            userResource?.Data.FirstName.Should().Be("John");
            userResource?.Data.LastName.Should().Be("Doe");
            userResource?.Data.DateOfBirth.Should().BeNull();
            userResource?.Data.Gender.Should().BeNull();
            userResource?.Data.Sex.Should().BeNull();
            userResource?.Links.Should().BeNull();
            userResource?.Embedded.Should().BeNull();
        }

        [Test]
        [Category("Happy")]
        public void Given_Valid_And_More_Complete_JSON___When_Deserialized_With_Converter___Then_Should_Produce_User_Data_Object()
        {
            // Arrange...
            var userId = Guid.NewGuid();
            var dob = "2001-02-03";
            var json = 
            "{" +
                $"\"Id\":\"{userId}\"," +
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
                    new Users.API.Models.Shared.UserResourceConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var userResource = JsonSerializer.Deserialize<Users.API.Models.Shared.UserResource>(json, options);

            // Assert...
            userResource.Should().NotBeNull();
            userResource?.Data.Should().NotBeNull();
            userResource?.Data.Id.Should().Be(userId);
            userResource?.Data.FirstName.Should().Be("John");
            userResource?.Data.LastName.Should().Be("Doe");
            userResource?.Data.DateOfBirth.Should().Be(dob);
            userResource?.Data.Gender.Should().Be(Users.Domain.Enumerations.Gender.Cisgender);
            userResource?.Data.Sex.Should().Be(Users.Domain.Enumerations.Sex.Male);
            userResource?.Links.Should().BeNull();
            userResource?.Embedded.Should().BeNull();
        }

        [Test]
        [Category("Happy")]
        public void Given_Valid_And_More_Complete_JSON_But_With_Empty_Values___When_Deserialized_With_Converter___Then_Should_Produce_User_Data_Object()
        {
            // Arrange...
            var userId = Guid.NewGuid();
            var json =
            "{" +
                $"\"Id\":\"{userId}\"," +
                "\"FirstName\":\"John\"," +
                "\"LastName\":\"Doe\"," +
                $"\"DateOfBirth\":\"\"," +
                "\"Sex\":\"\"," +
                "\"Gender\":\"\"" +
            "}";

            // Act...
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new Users.API.Models.Shared.UserResourceConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var userResource = JsonSerializer.Deserialize<Users.API.Models.Shared.UserResource>(json, options);

            // Assert...
            userResource.Should().NotBeNull();
            userResource?.Data.Should().NotBeNull();
            userResource?.Data.Id.Should().Be(userId);
            userResource?.Data.FirstName.Should().Be("John");
            userResource?.Data.LastName.Should().Be("Doe");
            userResource?.Data.DateOfBirth.Should().BeNull();
            userResource?.Data.Gender.Should().BeNull();
            userResource?.Data.Sex.Should().BeNull();
            userResource?.Links.Should().BeNull();
            userResource?.Embedded.Should().BeNull();
        }

        [Test]
        [Category("Happy")]
        public void Given_Valid_JSON_With_Links___When_Deserialized_With_Converter___Then_Should_Produce_User_Data_Object()
        {
            // Arrange...
            var userId = Guid.NewGuid();
            var json = 
            "{" +
                $"\"Id\":\"{userId}\"," +
                "\"FirstName\":\"John\"," +
                "\"LastName\":\"Doe\"," +
                "\"_links\":[" +
                    $"{{\"hypertextReference\":\"http://localhost/api/v1/users/{userId}\",\"relationship\":\"self\",\"method\":\"GET\"}}," +
                    $"{{\"hypertextReference\":\"http://localhost/api/v1/users/{userId}\",\"relationship\":\"self\",\"method\":\"DELETE\"}}" +
                "]" +
            "}";

            // Act...
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new Users.API.Models.Shared.UserResourceConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var userResource = JsonSerializer.Deserialize<Users.API.Models.Shared.UserResource>(json, options);

            // Assert...
            userResource.Should().NotBeNull();
            userResource?.Data.Should().NotBeNull();
            userResource?.Data.Id.Should().Be(userId);
            userResource?.Data.FirstName.Should().Be("John");
            userResource?.Data.LastName.Should().Be("Doe");
            userResource?.Data.DateOfBirth.Should().BeNull();
            userResource?.Data.Gender.Should().BeNull();
            userResource?.Data.Sex.Should().BeNull();
            userResource?.Links.Should().NotBeNull();
            userResource?.Links.Count.Should().Be(2);
            var getLink = userResource?.Links.SingleOrDefault(_ => _.Method == "GET" && _.Relationship == "self");
            getLink.Should().NotBeNull();
            getLink?.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");
            var deleteLink = userResource?.Links.SingleOrDefault(_ => _.Method == "DELETE" && _.Relationship == "self");
            deleteLink?.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");
            deleteLink.Should().NotBeNull();

            userResource?.Embedded.Should().BeNull();
        }

        [Test]
        [Category("Happy")]
        //[TestCase()]
        public void Given_Valid_JSON_With_Embedded___When_Deserialized_With_Converter___Then_Should_Produce_User_Data_Object()
        {
            // Arrange...
            var userId = Guid.NewGuid();
            var json =
            "{" +
                "\"_embedded\":[" +
                    "{" +
                        $"\"Id\":\"{userId}\"," +
                        "\"FirstName\":\"John\"," +
                        "\"LastName\":\"Doe\"," +
                        "\"_links\":[" +
                            $"{{\"hypertextReference\":\"http://localhost/api/v1/users/{userId}\",\"relationship\":\"self\",\"method\":\"GET\"}}," +
                            $"{{\"hypertextReference\":\"http://localhost/api/v1/users/{userId}\",\"relationship\":\"self\",\"method\":\"DELETE\"}}" +
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
                    new Users.API.Models.Shared.UserResourceConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var userResource = JsonSerializer.Deserialize<Users.API.Models.Shared.UserResource>(json, options);

            // Assert...
            userResource.Should().NotBeNull();
            userResource?.Data.Should().BeNull();
            userResource?.Links.Should().BeNull();
            
            userResource?.Embedded.Should().NotBeNull();
            userResource?.Embedded.Count.Should().Be(1);

            var embeddedUserData = userResource?.Embedded.FirstOrDefault();
            embeddedUserData.Should().NotBeNull();
            embeddedUserData?.Links.Count.Should().Be(2);
            embeddedUserData?.Data.Id.Should().Be(userId);
            embeddedUserData?.Data.FirstName.Should().Be("John");
            embeddedUserData?.Data.LastName.Should().Be("Doe");
            embeddedUserData?.Data.DateOfBirth.Should().BeNull();
            embeddedUserData?.Data.Gender.Should().BeNull();
            embeddedUserData?.Data.Sex.Should().BeNull();

            var getLink = embeddedUserData?.Links.SingleOrDefault(_ => _.Method == "GET" && _.Relationship == "self");
            getLink.Should().NotBeNull();
            getLink?.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");
            
            var deleteLink = embeddedUserData?.Links.SingleOrDefault(_ => _.Method == "DELETE" && _.Relationship == "self");
            deleteLink?.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");
            deleteLink.Should().NotBeNull();

        }

        [Test]
        [Category("Unhappy")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("InvalidValue")]
        public void Given_JSON_With_Invalid_User_ID___When_Deserialized_With_Converter___Then_Should_Throw_InvalidOperationException(string invalidUserId)
        {
            // Arrange...
            var json =
            "{" +
                $"\"Id\":\"{invalidUserId}\"," +
                "\"FirstName\":\"John\"," +
                "\"LastName\":\"Doe\"," +
            "}";

            // Act...
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new Users.API.Models.Shared.UserResourceConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Users.API.Models.Shared.UserResource>(json, options));

            // Assert...
            exception.Should().NotBeNull();
            exception?.Message.StartsWith("The JSON value could not be converted to Users.API.Models.Shared.UserResource.").Should().BeTrue();
            exception?.InnerException.Should().NotBeNull();
            exception?.InnerException?.Message.Should().Be("The JSON value is not in a supported Guid format.");



            /*
             * The JSON value could not be converted to Users.API.Models.Shared.UserResource.
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
            var userId = Guid.NewGuid();
            var json =
            "{" +
                $"\"Id\":\"{userId}\"," +
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
                    new Users.API.Models.Shared.UserResourceConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var exception = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<Users.API.Models.Shared.UserResource>(json, options));

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
            var userId = Guid.NewGuid();
            var json =
                "{" +
                $"\"Id\":\"{userId}\"," +
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
                    new Users.API.Models.Shared.UserResourceConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var exception = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<Users.API.Models.Shared.UserResource>(json, options));

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
            var userId = Guid.NewGuid();
            var json =
                "{" +
                $"\"Id\":\"{userId}\"," +
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
                    new Users.API.Models.Shared.UserResourceConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var exception = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<Users.API.Models.Shared.UserResource>(json, options));

            // Assert...
            exception?.Message.Should().Be($"Gender is not valid: Actual value: '{gender}'");
        }
    }
}
