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
        var sex = "[Null]";
        var gender = "[Null]";

        if (Sex != null) sex = Enum.GetName(typeof(Sex), Sex);
        if (Gender != null) gender = Enum.GetName(typeof(Gender), Gender);

        return $"Id:{Id}|FirstName:{FirstName}|LastName:{LastName}|Sex:{sex}|Gender:{gender}";
    }
}
