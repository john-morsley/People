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
        return $"Id:{Id}|FirstName:{FirstName}|LastName:{LastName}|Sex:{Enum.GetName(typeof(Sex), Sex)}|Gender:{Enum.GetName(typeof(Gender), Gender)}";
    }
}
