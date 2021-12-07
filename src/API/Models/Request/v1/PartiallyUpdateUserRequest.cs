namespace Users.API.Models.Request.v1;

public class PartiallyUpdateUserRequest
{
    private string _firstName;
    private string _lastName;
    private Sex? _sex;
    private Gender? _gender;

    public PartiallyUpdateUserRequest(Guid id)
    {
        Id = id;
    }
        
    public Guid Id { get; protected set; }

    public string FirstName
    {
        get => _firstName;
        set
        {
            FirstNameChanged = true;
            _firstName = value;
        }
    }

    public bool FirstNameChanged { get; private set; }

    public string LastName
    {
        get => _lastName;
        set
        {
            LastNameChanged = true;
            _lastName = value;
        }
    }

    public bool LastNameChanged { get; private set; }

    public Sex? Sex
    {
        get => _sex;
        set
        {
            SexChanged = true;
            _sex = value;
        }
    }

    public bool SexChanged { get; private set; }

    public Gender? GenderName
    {
        get => _gender;
        set
        {
            GenderChanged = true;
            _gender = value;
        }
    }

    public bool GenderChanged { get; private set; }
}
