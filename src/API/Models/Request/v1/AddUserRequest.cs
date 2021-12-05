namespace Users.API.Models.Request.v1;

public class AddUserRequest //: IValidatableObject
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    //{
    //    throw new NotImplementedException();
    //}
}
