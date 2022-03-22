namespace Morsley.UK.People.Persistence.Integration.Tests;

internal class Get : PersonRepositoryTests
{
    [Test]
    public async Task Getting_A_Person_Should_Result_In_That_Person_Being_Returned()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        var expected = GenerateTestPerson();
        AddPersonToDatabase(expected);
    
        NumberOfPeopleInDatabase().Should().Be(1);

        var sut = new PersonRepository(MongoContext!);

        // Act...
        var actual = await sut.GetByIdAsync(expected.Id);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(1);

        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task Getting_A_Page_Of_People_With_No_Pagination_Options_Should_Result_In_A_Page_Of_All_People_Being_Returned()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        var people = AddPeopleToDatabase(10);

        NumberOfPeopleInDatabase().Should().Be(10);

        var sut = new PersonRepository(MongoContext!);

        // Act...
        var getPeopleOptions = new GetOptions();
        var pageOfPeople = await sut.GetPageAsync(getPeopleOptions);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(10);

        pageOfPeople.Count().Should().Be(10);
        pageOfPeople.CurrentPage.Should().Be(1);
        pageOfPeople.TotalPages.Should().Be(1);
        pageOfPeople.TotalCount.Should().Be(10);
        pageOfPeople.HasPrevious.Should().BeFalse();
        pageOfPeople.HasNext.Should().BeFalse();

        ShouldBeEquivalentTo(pageOfPeople, people);
    }

    [Test]
    public async Task Getting_A_Page_Of_People_With_Pagination_Options_Should_Result_In_A_Page_Of_People_Being_Returned()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        var people = AddPeopleToDatabase(10);

        NumberOfPeopleInDatabase().Should().Be(10);

        var sut = new PersonRepository(MongoContext!);

        // Act...
        var getPeopleOptions = new GetOptions
        {
            PageSize = 1,
            PageNumber = 1
        };
        var pageOfPeople = await sut.GetPageAsync(getPeopleOptions);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(10);

        pageOfPeople.Count().Should().Be(1);
        pageOfPeople.CurrentPage.Should().Be(1);
        pageOfPeople.TotalPages.Should().Be(10);
        pageOfPeople.TotalCount.Should().Be(10);
        pageOfPeople.HasPrevious.Should().BeFalse();
        pageOfPeople.HasNext.Should().BeTrue();

        ShouldBeEquivalentTo(pageOfPeople, people);
    }

    [Test]
    public async Task Getting_A_Page_Of_People_With_Pagination_Options_Should_Result_In_The_Correct_Page_Of_People_Being_Returned()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        AddPeopleToDatabase(10);

        var people = AddPeopleToDatabase(10);

        AddPeopleToDatabase(10);

        NumberOfPeopleInDatabase().Should().Be(30);

        var sut = new PersonRepository(MongoContext!);

        // Act...
        var getPeopleOptions = new GetOptions
        {
            PageSize = 10,
            PageNumber = 2
        };
        var pageOfPeople = await sut.GetPageAsync(getPeopleOptions);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(30);

        pageOfPeople.Count().Should().Be(10);
        pageOfPeople.CurrentPage.Should().Be(2);
        pageOfPeople.TotalPages.Should().Be(3);
        pageOfPeople.TotalCount.Should().Be(30);
        pageOfPeople.HasPrevious.Should().BeTrue();
        pageOfPeople.HasNext.Should().BeTrue();

        ShouldBeEquivalentTo(pageOfPeople, people);
    }

    [Test]
    public async Task Getting_A_Page_Of_People_With_Search_Criteria_Should_Result_In_A_Page_Of_People_That_Match_The_Search_Being_Returned()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        AddPeopleToDatabase(5);

        var expected = new Person(Guid.NewGuid(), "John", "Morsley");
        AddPersonToDatabase(expected);

        AddPeopleToDatabase(5);

        NumberOfPeopleInDatabase().Should().Be(11);

        var sut = new PersonRepository(MongoContext!);

        // Act...
        var getPeopleOptions = new GetOptions
        {
            PageSize = 1,
            PageNumber = 1,
            Search = "orsle"
        };
        var pageOfPeople = await sut.GetPageAsync(getPeopleOptions);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(11);

        pageOfPeople.Count().Should().Be(1);
        pageOfPeople.CurrentPage.Should().Be(1);
        pageOfPeople.TotalPages.Should().Be(1);
        pageOfPeople.TotalCount.Should().Be(1);
        pageOfPeople.HasPrevious.Should().BeFalse();
        pageOfPeople.HasNext.Should().BeFalse();

        ShouldBeEquivalentTo(pageOfPeople, new List<Person>{ expected });
    }

    [Test]
    public async Task Getting_A_Page_Of_People_With_Filter_Criteria_Should_Result_In_A_Page_Of_People_That_Match_The_Filter()
    {
        // Arrange...
        NumberOfPeopleInDatabase().Should().Be(0);

        AddPeopleToDatabase(5);

        var john = new Person(Guid.NewGuid(), "John", "Doe");
        AddPersonToDatabase(john);

        var jane = new Person(Guid.NewGuid(), "Jane", "Doe");
        AddPersonToDatabase(jane);

        AddPeopleToDatabase(5);

        NumberOfPeopleInDatabase().Should().Be(12);

        var sut = new PersonRepository(MongoContext!);

        // Act...
        var getPeopleOptions = new GetOptions
        {
            PageSize = 2,
            PageNumber = 1
        };
        getPeopleOptions.AddFilter(new Filter("LastName", "Doe"));
        var pageOfPeople = await sut.GetPageAsync(getPeopleOptions);

        // Assert...
        NumberOfPeopleInDatabase().Should().Be(12);

        //pageOfPeople.Count().Should().Be(2);
        //var firstPersonOnPage = pageOfPeople.First();
        //firstPersonOnPage.Should().BeEquivalentTo(john);
        //var secondPersonOnPage = pageOfPeople.Skip(1).First();
        //secondPersonOnPage.Should().BeEquivalentTo(jane);
        ShouldBeEquivalentTo(pageOfPeople, new List<Person> { john, jane });
    }

    private void ShouldBeEquivalentTo(
        IPagedList<Person> pageOfPeople, 
        IList<Person> people)
    {
        foreach (var pagedPerson in pageOfPeople)
        {
            var person = people.Single(_ => _.Id == pagedPerson.Id);
            pagedPerson.Should().BeEquivalentTo(person);
        }
    }
}