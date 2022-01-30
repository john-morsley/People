namespace Users.API.Read.Tests.v1.Methods.GET.Single;

public class GetUserWithFields : APIsTestBase<StartUp>
{
    [Category("Happy")]
    [Test]    
    [TestCase("FirstName")]
    [TestCase("LastName")]
    [TestCase("Sex")]
    [TestCase("Gender")]
    [TestCase("DateOfBirth")]
    [TestCase("FirstName,LastName,Sex,Gender,DateOfBirth")]
    public async Task Given_User_Exist___When_It_Is_Requested_With_Fields___Then_200_OK_And_User_Should_Be_Shaped(string validFields)
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);
        var userId = Guid.NewGuid();
        var expected = GenerateTestUser(userId);
        AddUserToDatabase(expected);
        NumberOfUsersInDatabase().Should().Be(1);

        validFields = AddToFieldsIfMissing("Id", validFields);        

        // Act...
        var url = $"/api/v1/users/{userId}?fields={validFields}";
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var actual = DeserializeUser(content);
        actual.Should().NotBeNull();
        actual.Id.Should().Be(expected.Id);

        //actual.Should().BeEquivalentTo(expected, options => options.Using(new UserEquivalencyStep()));

        var expectedFields = new List<string>();
        var unexpectedFields = AllUserFields<Users.API.Models.Response.v1.UserResponse>();

        foreach (var expectedField in validFields.Split(','))
        {
            expectedFields.Add(expectedField);
            unexpectedFields.Remove(expectedField);
        }
        unexpectedFields.Remove("Links");
        ShouldBeEqual(expectedFields, actual, expected);
        ShouldBeNull(unexpectedFields, actual);

        var hateoas = DeserializeMetadata(content);
        hateoas.Links.Count().Should().Be(2);
        var getUserLink = hateoas.Links.Single(_ => _.Method == "GET");
        var deleteUserLink = hateoas.Links.Single(_ => _.Method == "DELETE");
    }

    [Test]
    [Category("UnHappy")]
    public async Task When_A_User_Is_Requested_With_Invalid_Fields___Then_422_UnprocessableEntity_And_Errors_Object_Should_Detail_Validation_Issues()
    {
        // Arrange...
        NumberOfUsersInDatabase().Should().Be(0);

        // Act...
        var userId = Guid.NewGuid();
        var url = $"/api/v1/users/{userId}?fields=fielddoesnotexist";
        var result = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);

        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var content = await result.Content.ReadAsStringAsync();
        content.Length.Should().BeGreaterThan(0);

        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(content);
        problemDetails.Should().NotBeNull();
        problemDetails.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        problemDetails.Title.Should().Be("Validation error(s) occurred!");
        problemDetails.Detail.Should().Be("See the errors field for details.");
        problemDetails.Instance.Should().Be($"/api/v1/users/{userId}");
        problemDetails.Extensions.Should().NotBeNull();
        var traceId = problemDetails.Extensions.Where(_ => _.Key == "traceId").FirstOrDefault();
        traceId.Should().NotBeNull();
        problemDetails.Errors.Count().Should().Be(1);
        var error = problemDetails.Errors.First();
        error.Key.Should().Be("Fields");
        var value = error.Value.First();
        value.Should().Be("The fields value is invalid. e.g. fields=id,lastname");
    }

    private static string AddToFieldsIfMissing(string toAdd, string fields)
    {
        var listOfFields = new List<string>();
        var hasId = false;
        foreach (var field in fields.Split(','))
        {
            if (field.ToLower() == toAdd.ToLower()) hasId = true;
            listOfFields.Add(field);
        }
        if (!hasId) listOfFields.Add(toAdd);
        return string.Join(",", listOfFields);
    }

    private IList<string> AllUserFields<T>() where T : class
    {
        var userFields = new List<string>();

        var propertyInfos = typeof(T).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        foreach (var propertyInfo in propertyInfos)
        {
            userFields.Add(propertyInfo.Name);
        }

        return userFields;
    }

    private object GetValue<T>(T t, string fieldName) where T : class
    {
        var propertyInfo = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        var type = propertyInfo.PropertyType;
        var value = propertyInfo.GetValue(t);
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null)
        {
            switch (underlying.FullName)
            {
                case "System.DateTime": return ((DateTime)value).ToString("yyyy-MM-dd");
            }
        }

        return value;
    }

    private void ShouldBeEqual(IList<string> fieldNames, Models.Response.v1.UserResponse actual, Domain.Models.User expected)
    {
        foreach (var fieldName in fieldNames)
        {
            ShouldBeEqual(fieldName, actual, expected);
        }
    }

    private void ShouldBeEqual(string fieldName, Models.Response.v1.UserResponse actual, Domain.Models.User expected)
    {
        var actualValue = GetValue(actual, fieldName);
        var expectedValue = GetValue(expected, fieldName);
        actualValue.Should().Be(expectedValue);
    }

    private void ShouldBeNull(IList<string> fieldNames, Models.Response.v1.UserResponse actual)
    {
        foreach (var fieldName in fieldNames)
        {
            ShouldBeNull(fieldName, actual);
        }
    }

    private void ShouldBeNull(string fieldName, Models.Response.v1.UserResponse actual)
    {
        var actualValue = GetValue(actual, fieldName);
        actualValue.Should().BeNull();
    }
}
