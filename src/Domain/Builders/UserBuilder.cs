namespace Users.Domain.Builders;

public class UserBuilder
{
    private string _firstName;
    private string _lastName;
    private Sex? _sex;
    private Gender? _gender;
    private DateTime? _dateOfBirth;

    public void SetFirstName(string value)
    {
        _firstName = value;
    }

    public void SetLastName(string value)
    {
        _lastName = value;
    }

    public void SetSex(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            _sex = null;
        }
        else
        {
            _sex = Enum.Parse<Sex>(value);
        }
    }

    public void SetGender(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            _gender = null;
        }
        else
        {
            _gender = Enum.Parse<Gender>(value);
        }
    }

    public void SetDateOfBirth(DateOnly value)
    {
        _dateOfBirth = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    public void SetDateOfBirth(string value)
    {
        var parts = value.Split('-');
        var year = int.Parse(parts[0]);
        var month = int.Parse(parts[1]);
        var day = int.Parse(parts[2]);
        _dateOfBirth = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
    }
    
    public Users.Domain.Models.User Build()
    {
        var user = new Users.Domain.Models.User(_firstName, _lastName)
        {
            Sex = _sex,
            Gender = _gender,
            DateOfBirth = _dateOfBirth
        };

        return user;
    }
}
