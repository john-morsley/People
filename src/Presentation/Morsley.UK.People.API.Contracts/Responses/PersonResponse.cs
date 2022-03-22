namespace Morsley.UK.People.API.Contracts.Responses;

public class PersonResponse
{
    public Guid Id { get; private set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Sex { get; set; }

    public string? Gender { get; set; }

    public string? DateOfBirth { get; set; }

    public PersonResponse(Guid id)
    {
        Id = id;
    }

    public override string ToString()
    {
        var firstName = FormatStringValue(FirstName);
        var lastName = FormatStringValue(LastName);
        var sex = FormatStringValue(Sex);
        var gender = FormatStringValue(Gender);
        var dateOfBirth = FormatStringValue(DateOfBirth);

        return $"Id: {Id} | FirstName: {firstName} | LastName: {lastName} | Sex: {sex} | Gender: {gender} | DateOfBirth: {dateOfBirth}";
    }

    private string FormatStringValue(string? value)
    {
        if (value == null) return "[Null]";
        if (!string.IsNullOrEmpty(FirstName) && FirstName.Length == 0) return "[Empty]";
        return value;
    }

    //private static string? FormatEnumValue<T>(T value)
    //{
    //    if (value == null) return "[Null]";
    //    var type = typeof(T);
    //    var underlying = Nullable.GetUnderlyingType(type);
    //    if (underlying != null) type = underlying;
    //    return Enum.GetName(type, value);
    //}
}
