namespace Morsley.UK.People.API.Contracts.Responses;

public class PersonResponse
{
    public Guid Id { get; private set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Sex { get; set; }

    public string? Gender { get; set; }

    public string? DateOfBirth { get; set; }

    //public string? Email { get; set; }

    //public string? Mobile { get; set; }

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
        //var email = FormatStringValue(Email);
        //var mobile = FormatStringValue(Mobile);

        return $"Id: {Id} | " +
               $"FirstName: {firstName} | " +
               $"LastName: {lastName} | " +
               $"Sex: {sex} | " +
               $"Gender: {gender} | " +
               $"DateOfBirth: {dateOfBirth}";
        //$"Email: { email} | " +
        //$"Mobile: { mobile}";
    }

    private string FormatStringValue(string? value)
    {
        if (value == null) return "[Null]";
        if (!string.IsNullOrEmpty(FirstName) && FirstName.Length == 0) return "[Empty]";
        return value;
    }
}
