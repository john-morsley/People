namespace Users.Domain.Models;

public class Country : Entity<Guid>
{
    public Country(string value) : base(Guid.NewGuid()) {}

    public string Value { get; set; }

    public string TwoLetterCode { get; set; }

    public string ThreeLetterCode { get; set; }
}