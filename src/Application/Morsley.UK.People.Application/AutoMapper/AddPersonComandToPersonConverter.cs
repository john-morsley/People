namespace Morsley.UK.People.Application.AutoMapper;

public class AddPersonComandToPersonConverter : ITypeConverter<AddPersonCommand, Person>
{
    public Person Convert(
        AddPersonCommand source,
        Person destination,
        ResolutionContext context)
    {
        if (source.Id == Guid.Empty) throw new ArgumentNullException(nameof(source), "source.Id cannot be empty!");
        if (string.IsNullOrWhiteSpace(source.FirstName)) throw new ArgumentException("source.FirstName cannot be null, empty or whitespace!", nameof(source));
        if (string.IsNullOrWhiteSpace(source.LastName)) throw new ArgumentException("source.LastName cannot be null, empty or whitespace!", nameof(source));

        Person? person;
        if (source.Id is null)
        {
            person = new Person(source.FirstName, source.LastName);
        }
        else
        {
            person = new Person(source.Id.Value, source.FirstName, source.LastName);
        }

        person.Sex = source.Sex;
        person.Gender = source.Gender;

        if (DateTime.TryParseExact(source.DateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            person.DateOfBirth = result;
        }

        return person;
    }
}