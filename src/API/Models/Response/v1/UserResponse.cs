namespace Users.API.Models.Response.v1;

public class UserResponse
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }

    public string DateOfBirth { get; set; }

    //public IEnumerable<Users.API.Models.Shared.Link> Links { get; set; }

    public override string ToString()
    {
        var firstName = FormatStringValue(FirstName);
        var lastName = FormatStringValue(LastName);
        var sex = FormatEnumValue(Sex);
        var gender = FormatEnumValue(Gender);
        var dateOfBirth = FormatStringValue(DateOfBirth);

        return $"Id: {Id} | FirstName: {firstName} | LastName: {lastName} | Sex: {sex} | Gender: {gender} | DateOfBirth: {dateOfBirth}";
    }

    private string FormatStringValue(string value)
    {
        if (value == null) return "[Null]";
        if (!string.IsNullOrEmpty(FirstName) && FirstName.Length == 0) return "[Empty]";
        return value;
    }

    private string FormatEnumValue<T>(T value)
    {
        if (value == null) return "[Null]";
        var type = typeof(T);
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null) type = underlying;
        return Enum.GetName(type, value);
    }
}
