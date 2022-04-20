namespace Morsley.UK.People.API.SampleConsumer;

public class Person
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public override string ToString()
    {
        return $"First Name: '{FirstName}' | Last Name: '{LastName}'";
    }
}
