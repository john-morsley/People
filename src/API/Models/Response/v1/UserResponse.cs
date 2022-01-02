namespace Users.API.Models.Response.v1;

public class UserResponse
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }

    public string DateOfBirth { get; set; }

    public override string ToString()
    {
        var firstName = "[Null]";
        var lastName = "[Null]";
        var sex = "[Null]";
        var gender = "[Null]";
        var dateOfBirth = "[Null]";

        if (FirstName == string.Empty) firstName = "[Empty]";
        if (LastName == string.Empty) lastName = "[Empty]";
        if (Sex != null) sex = Enum.GetName(typeof(Sex), Sex);
        if (Gender != null) gender = Enum.GetName(typeof(Gender), Gender);
        if (DateOfBirth == string.Empty)
        {
            dateOfBirth = "[Empty]";
        }
        else if (DateOfBirth.Length > 0) 
        { 
            dateOfBirth = DateOfBirth; 
        }

        return $"Id:{Id}|FirstName:{firstName}|LastName:{lastName}|Sex:{sex}|Gender:{gender}|DateOfBirth:{dateOfBirth}";
    }
}
