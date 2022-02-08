using System.Dynamic;
using System.Linq;
using System.Text.Json.Serialization;

namespace API.Models.Shared.Tests
{
    public class UserDataConverterTests
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
        public void Given_Valid_But_Minimal_JSON___When_Deserialized_With_Converter___Then_Should_Produce_User_Data_Object()
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
                    new Users.API.Models.Shared.UserDataConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var userData = JsonSerializer.Deserialize<Users.API.Models.Shared.UserData>(json, options);

            // Assert...
            userData.Should().NotBeNull();
            userData.User.Should().NotBeNull();
            userData.User.Id.Should().Be(userId);
            userData.User.FirstName.Should().Be("John");
            userData.User.LastName.Should().Be("Doe");
            userData.User.DateOfBirth.Should().BeNull();
            userData.User.Gender.Should().BeNull();
            userData.User.Sex.Should().BeNull();
            userData.Links.Should().BeNull();
            userData.Embedded.Should().BeNull();
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
                    new Users.API.Models.Shared.UserDataConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var userData = JsonSerializer.Deserialize<Users.API.Models.Shared.UserData>(json, options);

            // Assert...
            userData.Should().NotBeNull();
            userData.User.Should().NotBeNull();
            userData.User.Id.Should().Be(userId);
            userData.User.FirstName.Should().Be("John");
            userData.User.LastName.Should().Be("Doe");
            userData.User.DateOfBirth.Should().Be(dob);
            userData.User.Gender.Should().Be(Users.Domain.Enumerations.Gender.Cisgender);
            userData.User.Sex.Should().Be(Users.Domain.Enumerations.Sex.Male);
            userData.Links.Should().BeNull();
            userData.Embedded.Should().BeNull();
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
                    new Users.API.Models.Shared.UserDataConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var userData = JsonSerializer.Deserialize<Users.API.Models.Shared.UserData>(json, options);

            // Assert...
            userData.Should().NotBeNull();
            userData.User.Should().NotBeNull();
            userData.User.Id.Should().Be(userId);
            userData.User.FirstName.Should().Be("John");
            userData.User.LastName.Should().Be("Doe");
            userData.User.DateOfBirth.Should().BeNull();
            userData.User.Gender.Should().BeNull();
            userData.User.Sex.Should().BeNull();
            userData.Links.Should().BeNull();
            userData.Embedded.Should().BeNull();
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
                    new Users.API.Models.Shared.UserDataConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var userData = JsonSerializer.Deserialize<Users.API.Models.Shared.UserData>(json, options);

            // Assert...
            userData.Should().NotBeNull();
            userData.User.Should().NotBeNull();
            userData.User.Id.Should().Be(userId);
            userData.User.FirstName.Should().Be("John");
            userData.User.LastName.Should().Be("Doe");
            userData.User.DateOfBirth.Should().BeNull();
            userData.User.Gender.Should().BeNull();
            userData.User.Sex.Should().BeNull();
            userData.Links.Should().NotBeNull();
            userData.Links.Count.Should().Be(2);
            var getLink = userData.Links.SingleOrDefault(_ => _.Method == "GET" && _.Relationship == "self");
            getLink.Should().NotBeNull();
            getLink.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");
            var deleteLink = userData.Links.SingleOrDefault(_ => _.Method == "DELETE" && _.Relationship == "self");
            deleteLink.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");
            deleteLink.Should().NotBeNull();

            userData.Embedded.Should().BeNull();
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
                    new Users.API.Models.Shared.UserDataConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var userData = JsonSerializer.Deserialize<Users.API.Models.Shared.UserData>(json, options);

            // Assert...
            userData.Should().NotBeNull();
            userData.User.Should().BeNull();
            userData.Links.Should().BeNull();
            
            userData.Embedded.Should().NotBeNull();
            userData.Embedded.Count.Should().Be(1);

            var embeddedUserData = userData.Embedded.FirstOrDefault();
            embeddedUserData.Should().NotBeNull();
            embeddedUserData.Links.Count.Should().Be(2);
            embeddedUserData.User.Id.Should().Be(userId);
            embeddedUserData.User.FirstName.Should().Be("John");
            embeddedUserData.User.LastName.Should().Be("Doe");
            embeddedUserData.User.DateOfBirth.Should().BeNull();
            embeddedUserData.User.Gender.Should().BeNull();
            embeddedUserData.User.Sex.Should().BeNull();

            var getLink = embeddedUserData.Links.SingleOrDefault(_ => _.Method == "GET" && _.Relationship == "self");
            getLink.Should().NotBeNull();
            getLink.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");
            var deleteLink = embeddedUserData.Links.SingleOrDefault(_ => _.Method == "DELETE" && _.Relationship == "self");
            deleteLink.HypertextReference.Should().Be($"http://localhost/api/v1/users/{userId}");
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
                    new Users.API.Models.Shared.UserDataConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Users.API.Models.Shared.UserData>(json, options));

            // Assert...
            exception.Should().NotBeNull();
            exception.Message.StartsWith("The JSON value could not be converted to Users.API.Models.Shared.UserData.").Should().BeTrue();
            exception.InnerException.Should().NotBeNull();
            exception.InnerException.Message.Should().Be("The JSON value is not in a supported Guid format.");



            /*
             * The JSON value could not be converted to Users.API.Models.Shared.UserData.
             * 12345678901234567890123456789012345678901234567890123456789012345678901234
             *          1         2         3         4         5         6         7
             *
             *
             * The JSON value is not in a supported Guid format.
             */

        }

        [Test]
        [Category("Unhappy")]
        //[TestCase(null)]
        //[TestCase("")]
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
                    new Users.API.Models.Shared.UserDataConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var exception = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<Users.API.Models.Shared.UserData>(json, options));

            // Assert...
            exception.Message.Should().Be($"DateOfBirth is not valid: Expected format is 'YYYY-MM-DD', actual value: '{invalidDob}'");
        }

        [Test]
        [Category("Unhappy")]
        //[TestCase(null)]
        //[TestCase("")]
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
                    new Users.API.Models.Shared.UserDataConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var exception = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<Users.API.Models.Shared.UserData>(json, options));

            // Assert...
            exception.Message.Should().Be($"Sex is not valid: Actual value: '{sex}'");
        }

        [Test]
        [Category("Unhappy")]
        //[TestCase(null)]
        //[TestCase("")]
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
                    new Users.API.Models.Shared.UserDataConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var exception = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<Users.API.Models.Shared.UserData>(json, options));

            // Assert...
            exception.Message.Should().Be($"Gender is not valid: Actual value: '{gender}'");
        }
    }
}
