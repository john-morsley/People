namespace Users.API.Read.Tests.v1.Methods;

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

        validFields = AddIdToFieldsIfMissing(validFields);

        // Act...
        var url = $"/api/v1/users/{userId}?fields={validFields}";
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);

        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);

        var actual = DeserializeUserResponse(response);
        actual.Should().NotBeNull();
        actual.Id.Should().Be(expected.Id);

        var expectedFields = new List<string>();
        var unexpectedFields = AllUserFields();

        foreach (var expectedField in validFields.Split(','))
        {
            expectedFields.Add(expectedField);
            unexpectedFields.Remove(expectedField);
        }
        ShouldBeEqual(expectedFields, actual, expected);
        ShouldBeNull(unexpectedFields, actual);
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

    private static string AddIdToFieldsIfMissing(string fields)
    {
        var listOfFields = new List<string>();
        var hasId = false;
        foreach (var field in fields.Split(','))
        {
            if (field.ToLower() == "id") hasId = true;
            listOfFields.Add(field);
        }
        if (!hasId) listOfFields.Add("Id");
        return string.Join(",", listOfFields);
    }

    private IList<string> AllUserFields()
    {
        var userFields = new List<string>();

        var propertyInfos = typeof(Users.API.Models.Response.v1.UserResponse).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        foreach(var propertyInfo in propertyInfos)
        {
            userFields.Add(propertyInfo.Name);
        }

        return userFields;
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
        var httpResponse = await _client.GetAsync(url);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);

        httpResponse.IsSuccessStatusCode.Should().BeFalse();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var response = await httpResponse.Content.ReadAsStringAsync();
        response.Length.Should().BeGreaterThan(0);

        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(response);
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
}
