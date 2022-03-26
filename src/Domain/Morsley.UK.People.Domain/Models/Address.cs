namespace Morsley.UK.People.Domain.Models;

public class Address
{
    //public IList<string> Lines { get; set; }

    public string County { get; set; }

    public string PostCode { get; set; }

    public Country Country { get; set; }

    public bool IsPrimary { get; set; }
}