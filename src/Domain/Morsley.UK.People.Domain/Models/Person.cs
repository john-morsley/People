namespace Morsley.UK.People.Domain.Models;

public class Person : AuditableEntity<Guid>
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private DateTime? _dateOfBirth;

    private readonly IList<Address> _addresses = new List<Address>();
    private readonly IList<Email> _emails = new List<Email>();
    private readonly IList<Phone> _phones = new List<Phone>();

    public Person() : base(Guid.NewGuid()) {}

    public Person(string firstName, string lastName) : base(Guid.NewGuid())
    {
        SetFirstName(firstName);
        SetLastName(lastName);
    }

    public Person(Guid id) : base(id)
    {
    }

    public Person(Guid id, string firstName, string lastName) : base(id)
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

    public DateTime? DateOfBirth
    {
        get => _dateOfBirth;
        set {
            if (!value.HasValue)
            {
                _dateOfBirth = null;
            }
            else
            {
                _dateOfBirth = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day, 0, 0, 0, 0, DateTimeKind.Utc);
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

    public override string ToString()
    {
        var firstName = FormatStringValue(FirstName);
        var lastName = FormatStringValue(LastName);
        var sex = FormatEnumValue(Sex);
        var gender = FormatEnumValue(Gender);
        var dateOfBirth = FormatDateTimeValue(DateOfBirth);

        return $"Id: {Id} | FirstName: {firstName} | LastName: {lastName} | Sex: {sex} | Gender: {gender} | DateOfBirth: {dateOfBirth}";
    }

    private string FormatStringValue(string value)
    {
        if (!string.IsNullOrEmpty(FirstName) && FirstName.Length == 0) return "[Empty]";
        return value;
    }

    private static string? FormatEnumValue<T>(T value)
    {
        if (value == null) return "[Null]";
        var type = typeof(T);
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null) type = underlying;
        return Enum.GetName(type, value);
    }

    private static string FormatDateTimeValue(DateTime? dt)
    {
        if (dt == null) return "[Null]";
        return $"{dt.Value.Year:0000}-{dt.Value.Month:00}-{dt.Value.Day:00}";
    }

    public static string GetCacheKey(Guid personId)
    {
        return $"Person-->Id:{personId}";
    }
}