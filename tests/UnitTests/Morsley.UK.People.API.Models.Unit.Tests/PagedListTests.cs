using Morsley.UK.People.API.Contracts.Responses;
using PersonResponse = Morsley.UK.People.API.Contracts.Responses.PersonResponse;

namespace Morsley.UK.People.API.Models.Tests
{
    public class PagedListTests
    {
        private readonly Fixture _autoFixture;

        private enum TestPeople
        {
            Adam,
            Fabio,
            Glen,
            John,
            Matt,
            Paul,
            Thomas
        }

        public PagedListTests()
        {
            _autoFixture = new Fixture();
        }

        //[Test]
        //public void Given_A_Paged_List___When_It_Is_Empty___Then_All_Properties_Should_Their_Defaults()
        //{
        //    // Act...
        //    var sut = GetPagedList();

        //    // Assert...
        //    sut.IsReadOnly.Should().BeFalse();
        //    sut.HasPrevious.Should().BeFalse();
        //    sut.HasNext.Should().BeFalse();
        //    sut.Count.Should().Be(0);
        //    sut.CurrentPage.Should().Be(0);
        //    sut.TotalPages.Should().Be(0);
        //    //sut.TotalCount.Should().Be(0);
        //    sut.PageSize.Should().Be(0);
        //}

        //[Test]
        //public void Given_A_Paged_List___When_It_has_One_Person___Then_Everything_Should_Be_Default()
        //{
        //    // Arrange...
        //    var sut = GetPagedList(1);

        //    // Act...
        //    sut.PageSize = 1;
        //    sut.CurrentPage = 1;
        //    sut.TotalPages = 1;

        //    // Assert...
        //    sut.IsReadOnly.Should().BeFalse();
        //    sut.HasPrevious.Should().BeFalse();
        //    sut.HasNext.Should().BeFalse();
        //    sut.Count.Should().Be(1);
        //    sut.CurrentPage.Should().Be(1);
        //    sut.TotalPages.Should().Be(1);
        //    //sut.TotalCount.Should().Be(0);
        //    sut.PageSize.Should().Be(1);
        //}

        //[Test]
        //public void Given_A_Paged_List___When_It_has_Two_Users___Then_Everything_Should_Be_Default()
        //{
        //    // Arrange...
        //    var sut = GetPagedList(2);

        //    // Act...
        //    sut.PageSize = 1;
        //    sut.CurrentPage = 1;
        //    sut.TotalPages = 2;            

        //    // Assert...
        //    sut.IsReadOnly.Should().BeFalse();
        //    sut.HasPrevious.Should().BeFalse();
        //    sut.HasNext.Should().BeTrue();
        //    sut.Count.Should().Be(2);
        //    sut.CurrentPage.Should().Be(1);
        //    sut.TotalPages.Should().Be(2);
        //    //sut.TotalCount.Should().Be(0);
        //    sut.PageSize.Should().Be(1);
        //}

        //[Test]
        //public void Given_A_Paged_List___When_It_has_Three_Users___Then_Everything_Should_Be_Default()
        //{
        //    // Arrange...
        //    var sut = GetPagedList(3);

        //    // Act...
        //    sut.PageSize = 1;
        //    sut.CurrentPage = 2;
        //    sut.TotalPages = 3;

        //    // Assert...
        //    sut.IsReadOnly.Should().BeFalse();
        //    sut.HasPrevious.Should().BeTrue();
        //    sut.HasNext.Should().BeTrue();
        //    sut.Count.Should().Be(3);
        //    sut.CurrentPage.Should().Be(2);
        //    sut.TotalPages.Should().Be(3);
        //    //sut.TotalCount.Should().Be(0);
        //    sut.PageSize.Should().Be(1);
        //}

        //[Test]
        //public void Given_An_Empty_Paged_List___When_Serializing___Then_The_Resultant_JSON_Should_Be_Valid()
        //{
        //    // Arrange...
        //    var sut = GetPagedList();

        //    // Act...
        //    var serializeOptions = new JsonSerializerOptions
        //    {
        //        WriteIndented = true,
        //        Converters =
        //        {                    
        //            new PagedListJsonConverter()
        //        }
        //    };
        //    var actualJson = JsonSerializer.Serialize(sut, serializeOptions);

        //    // Assert...
        //    actualJson.Should().NotBeNull();
        //    var expectedJson = GetJsonFromFile("PagedListEmpty.json");
        //    actualJson.Should().Be(expectedJson);
        //}

        //[Test]
        //public void Given_A_Paged_List_With_One_User___When_Serializing___Then_The_Resultant_JSON_Should_Be_Valid()
        //{
        //    // Arrange...
        //    var john = GetUserResponse(TestPeople.John);
        //    var pagedList = GetPagedList(john);
        //    pagedList.CurrentPage = 1;
        //    pagedList.PageSize = 1;
        //    pagedList.TotalPages = 1;
        //    pagedList.TotalCount = 1;

        //    // Act...
        //    var serializeOptions = new JsonSerializerOptions
        //    {
        //        Converters =
        //        {
        //            new PagedListJsonConverter(),
        //            new System.Text.Json.Serialization.JsonStringEnumConverter()
        //        }
        //    };
        //    var actualJson = CleanJson(JsonSerializer.Serialize(pagedList, serializeOptions));

        //    // Assert...
        //    actualJson.Should().NotBeNull();
        //    var expectedJson = CleanJson(GetJsonFromFile("PagedListOneUserJohn.json"));
        //    actualJson.Should().Be(expectedJson);
        //}

        //[Test]
        //public void Given_A_Paged_List_With_Three_User___When_Serializing___Then_The_Resultant_JSON_Should_Be_Valid()
        //{
        //    // Arrange...
        //    var john = GetUserResponse(TestPeople.John);
        //    var fabio = GetUserResponse(TestPeople.Fabio);
        //    var glen = GetUserResponse(TestPeople.Glen);
        //    var pagedList = GetPagedList(new List<PersonResponse>() { john, fabio, glen });
        //    pagedList.CurrentPage = 2;
        //    pagedList.PageSize = 1;
        //    pagedList.TotalPages = 3;
        //    pagedList.TotalCount = 3;

        //    // Act...
        //    var serializeOptions = new JsonSerializerOptions
        //    {
        //        Converters =
        //        {
        //            new PagedListJsonConverter(),
        //            new System.Text.Json.Serialization.JsonStringEnumConverter()
        //        }
        //    };
        //    var actualJson = CleanJson(JsonSerializer.Serialize(pagedList, serializeOptions));

        //    // Assert...
        //    actualJson.Should().NotBeNull();
        //    var expectedJson = CleanJson(GetJsonFromFile("PagedListThreeUsersJohnFabioGlen.json"));
        //    actualJson.Should().Be(expectedJson);
        //}

        //[Test]
        //public void Given_A_Paged_List_With_Three_Users___When_Serializing___Then_The_Resultant_JSON_Should_Be_Valid()
        //{
        //    // Arrange...
        //    var pagedList = GetPagedList(3);

        //    // Act...
        //    var serializeOptions = new JsonSerializerOptions
        //    {
        //        Converters =
        //        {
        //            new PagedListJsonConverter(),
        //            new System.Text.Json.Serialization.JsonStringEnumConverter()
        //        }
        //    };
        //    var json = JsonSerializer.Serialize(pagedList, serializeOptions);

        //    // Assert...
        //    json.Should().NotBeNull();
        //}

        //[Test]
        //public void JSON()
        //{
        //    // Arrange...
        //    var id1 = _autoFixture.Create<Guid>();
        //    var firstName1 = _autoFixture.Create<string>();
        //    var lastName1 = _autoFixture.Create<string>();
        //    var user1 = GetUserResponse(id1, firstName1, lastName1);
        //    var id2 = _autoFixture.Create<Guid>();
        //    var firstName2 = _autoFixture.Create<string>();
        //    var lastName2 = _autoFixture.Create<string>();
        //    var user2 = GetUserResponse(id2, firstName2, lastName2);
        //    var id3 = _autoFixture.Create<Guid>();
        //    var firstName3 = _autoFixture.Create<string>();
        //    var lastName3 = _autoFixture.Create<string>();
        //    var user3 = GetUserResponse(id3, firstName3, lastName3);
        //    var sut = GetPagedList(new List<PersonResponse>() { user1, user2, user3 });

        //    // Act...
        //    var serializeOptions = new JsonSerializerOptions
        //    {
        //        Converters =
        //        {
        //            new PagedListJsonConverter()
        //        }
        //    };
        //    var json = JsonSerializer.Serialize(sut, serializeOptions);
        //}

        //[Test]
        //public void Given_Serialized_JSON___When_Deserializing___Then_An_Empty_Paged_List_Should_Result()
        //{
        //    // Arrange...
        //    var json = GetJsonFromFile("PagedListEmpty.json");

        //    // Act...
        //    var deserializeOptions = new JsonSerializerOptions();
        //    deserializeOptions.Converters.Add(new PagedListJsonConverter());
        //    var pagedList = JsonSerializer.Deserialize<PagedList<PersonResponse>>(json, deserializeOptions);

        //    // Assert...
        //    pagedList.Should().NotBeNull();
        //    pagedList?.Count.Should().Be(0);
        //    pagedList?.IsReadOnly.Should().BeFalse();
        //    pagedList?.HasPrevious.Should().BeFalse();
        //    pagedList?.HasNext.Should().BeFalse();
        //    pagedList?.Count.Should().Be(0);
        //    pagedList?.CurrentPage.Should().Be(0);
        //    pagedList?.TotalPages.Should().Be(0);
        //    pagedList?.PageSize.Should().Be(0);
        //}

        //[Test]
        //public void Given_Serialized_JSON___When_Deserializing___Then_A_Paged_List_With_One_User_Should_Result()
        //{
        //    // Arrange...
        //    var json = GetJsonFromFile("PagedListOneUserJohn.json");

        //    // Act...
        //    var deserializeOptions = new JsonSerializerOptions
        //    {
        //        Converters =
        //        {
        //            new PagedListJsonConverter(),
        //            new System.Text.Json.Serialization.JsonStringEnumConverter()
        //        }
        //    };        
        //    var pagedList = JsonSerializer.Deserialize<PagedList<PersonResponse>>(json, deserializeOptions);

        //    // Assert...
        //    pagedList.Should().NotBeNull();
        //    pagedList?.Count.Should().Be(1);
        //    pagedList?.IsReadOnly.Should().BeFalse();
        //    pagedList?.HasPrevious.Should().BeFalse();
        //    pagedList?.HasNext.Should().BeFalse();
        //    pagedList?.CurrentPage.Should().Be(1);
        //    pagedList?.TotalPages.Should().Be(1);
        //    pagedList?.PageSize.Should().Be(1);
        //}

        //[Test]
        //public void Given_Serialized_JSON___When_Deserializing___Then_A_Paged_List_With_Three_Users_Should_Result()
        //{
        //    // Arrange...
        //    var json = GetJsonFromFile("PagedListThreeUsersJohnFabioGlen.json");

        //    // Act...            
        //    var deserializeOptions = new JsonSerializerOptions
        //    {
        //        Converters =
        //        {
        //            new PagedListJsonConverter(),
        //            new System.Text.Json.Serialization.JsonStringEnumConverter()
        //        }
        //    };
        //    var pagedList = JsonSerializer.Deserialize<PagedList<PersonResponse>>(json, deserializeOptions);

        //    // Assert...
        //    pagedList.Should().NotBeNull();
        //    pagedList?.Count.Should().Be(3);
        //    pagedList?.IsReadOnly.Should().BeFalse();
        //    pagedList?.HasPrevious.Should().BeTrue();
        //    pagedList?.HasNext.Should().BeTrue();
        //    pagedList?.CurrentPage.Should().Be(2);
        //    pagedList?.TotalPages.Should().Be(3);
        //    pagedList?.PageSize.Should().Be(1);
        //}

        //private PagedList<PersonResponse> GetPagedList(int numberOfUsers = 0)
        //{
        //    var pagedList = new PagedList<PersonResponse>();
        //    //sut.Add(new PersonResponse() { });
        //    for (int i = 0; i < numberOfUsers; i++)
        //    {
        //        var people = GetUserResponse();
        //        pagedList.Add(people);
        //    }
        //    return pagedList;
        //}

        /*
        {"CurrentPage":0,"PageSize":0,"TotalPages":0,"TotalCount":0,"HasPrevious":false,"HasNext":false,"Items":[{"Id":"d7ab6ae7-cbd0-4b5f-acfc-f109d30ba5e9","FirstName":"c987e770-2190-4f38-a029-5a07ca018fd9","LastName":"9a12e13b-d1b5-4a49-babc-7c7ee4417c83"}]}
            */

        /*
            {"CurrentPage":0,"PageSize":0,"TotalPages":0,"HasPrevious":false,"HasNext":false,"Items":[{"Id":"384cb2d8-e4e0-4478-bab5-bcaf730b8744","FirstName":"bd12ee92-e3db-49ed-b820-f31ea9e25daa","LastName":"5442b299-d2a3-4ed7-b484-60971698c130"},{"Id":"7e909be2-552f-4ccf-8b58-0de867b9f58f","FirstName":"3b1c67a8-04d1-4cce-a1e7-7fb39bb50ba5","LastName":"5212933a-8690-40ac-9263-9f1bb1608bda"},{"Id":"91cbd9aa-0730-4866-b08c-e57a0b45fa73","FirstName":"ab141a15-d1d2-42f2-a8cb-a3febf671ba0","LastName":"5208ba9d-404f-49f1-b6e2-3ad5d4e3a2af"}]}
            */

        private string GetJsonFromFile(string fileName)
        {
            var workingDirectory = Directory.GetCurrentDirectory();
            var projectDirectory = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName;
            if (projectDirectory == null) throw new InvalidOperationException("Cannot determine project directory.");
            var fullPathAndFileName = Path.Combine(projectDirectory, "JSON", fileName);
            var json = File.ReadAllText(fullPathAndFileName);
            return json;
        }

        private PagedList<PersonResponse> GetPagedList(PersonResponse person)
        {
            var people = new List<PersonResponse>();
            people.Add(person);
            return GetPagedList(people);
        }

        private PagedList<PersonResponse> GetPagedList(IEnumerable<PersonResponse> people)
        {
            //var pagedList = PagedList<PersonResponse>.Create(people, 1, 1);

            var pagedList = new PagedList<PersonResponse>(people);

            return pagedList;
        }

        private PersonResponse GetPersonResponse()
        {
            return _autoFixture.Create<PersonResponse>();
        }

        private PersonResponse GetPersonResponse(Guid id, string firstName, string lastName, Sex? sex, Gender? gender)
        {
            return new PersonResponse(id)
            {
                FirstName = firstName, 
                LastName = lastName,
                Sex = sex?.ToString(), 
                Gender = gender?.ToString()
            };
        }

        private PersonResponse GetPersonResponse(TestPeople people)
        {
            switch (people)
            {
                case TestPeople.John: return GetPersonResponse(Guid.Parse("11111111-1111-1111-1111-111111111111"), "John", "Morsley", Sex.Male, Gender.Cisgender);
                case TestPeople.Fabio: return GetPersonResponse(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Fabio", "Sereno", Sex.Male, Gender.Cisgender);
                case TestPeople.Glen: return GetPersonResponse(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Glen", "Clark", Sex.Male, Gender.Cisgender);
                case TestPeople.Adam: return GetPersonResponse(Guid.Parse("44444444-4444-4444-4444-444444444444"), "Adam", "Bray", Sex.Male, Gender.Cisgender);
                case TestPeople.Matt: return GetPersonResponse(Guid.Parse("55555555-5555-5555-5555-555555555555"), "Matt", "Dumont", Sex.Male, Gender.Cisgender);
                case TestPeople.Paul: return GetPersonResponse(Guid.Parse("66666666-6666-6666-6666-666666666666"), "Paul", "Martin", Sex.Male, Gender.Cisgender);
                case TestPeople.Thomas: return GetPersonResponse(Guid.Parse("77777777-7777-7777-7777-777777777777"), "Thomas", "Grimshaw", Sex.Male, Gender.Cisgender);
            }
            throw new NotImplementedException("This should never happen!");
        }

        private string CleanJson(string json)
        {            
            return Newtonsoft.Json.JsonConvert.SerializeObject(Newtonsoft.Json.JsonConvert.DeserializeObject(json));
        }
    }
}