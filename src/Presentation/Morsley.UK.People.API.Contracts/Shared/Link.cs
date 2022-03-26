namespace Morsley.UK.People.API.Contracts.Shared;

public class Link
{
    public string HypertextReference { get; }

    public string Relationship { get; }

    public string Method { get; }

    public Link(string hypertextReference, string relationship, string method)
    {
        HypertextReference = hypertextReference;
        Relationship = relationship;
        Method = method;
    }
}