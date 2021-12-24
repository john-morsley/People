namespace Users.Domain.Models;

public class User : Entity<Guid>
{
    private string _firstName;
    private string _lastName;
    //private DateOfBirth _dateOfBirth;
    private DateTime? _dateOfBirth;

    private IList<Address> _addresses;
    private IList<Email> _emails;
    private IList<Phone> _phones;

    public User()
    {
        _id = Guid.NewGuid();
        _addresses = new List<Address>();
        _emails = new List<Email>();
        _phones = new List<Phone>();
    }

    public User(string firstName, string lastName) : this()
    {
        SetFirstName(firstName);
        SetLastName(lastName);
    }

    public string FirstName
    {
        get => _firstName;
        set => SetFirstName(value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetLastName(value);
    }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }

    //public DateOfBirth DateOfBirth
    //{
    //    get => _dateOfBirth;
    //    set => _dateOfBirth = value;
    //}

    //public DateOnly? DateOfBirth
    //{
    //    get => _dateOfBirth;
    //    set => _dateOfBirth = value;
    //}

    public DateTime? DateOfBirth 
    {
        get => _dateOfBirth;
        set
        {
            if (!value.HasValue)
            {
                _dateOfBirth = null;
            }
            else
            {
                _dateOfBirth = value;
            }
        }
    }

    public IReadOnlyList<Email> Emails => _emails.ToList();

    public IReadOnlyList<Phone> Phones => _phones.ToList();

    public IReadOnlyList<Address> Addresses => _addresses.ToList();

    public void AddAddress(Address newAddress)
    {
        // Make sure only one address is primary...
        // ToDo

        _addresses.Add(newAddress);
    }

    public void AddEmail(Email newEmail)
    {
        // Make sure only one email is primary...
        // ToDo

        _emails.Add(newEmail);
    }

    public void AddPhone(Phone newPhone)
    {
        // Make sure only one phone is primary...
        // ToDo

        _phones.Add(newPhone);
    }

    private void SetFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("Cannot be empty!", nameof(firstName));
        _firstName = firstName;
    }

    private void SetLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Cannot be empty!", nameof(lastName));
        _lastName = lastName;
    }
}