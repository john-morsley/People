namespace Users.API.Models.Request.v1;

public class AddUserRequest
{
    public AddUserRequest(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }

    public string? DateOfBirth { get; set; }
}
