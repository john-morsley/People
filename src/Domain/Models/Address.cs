namespace Users.Domain.Models;

public class Address : Entity<Guid>
{
    public Address() : base(Guid.NewGuid()) { }

    //public IList<string> Lines { get; set; }

    public string County { get; set; }

    public string PostCode { get; set; }

    public Country Country { get; set; }

    public bool IsPrimary { get; set; }
}
