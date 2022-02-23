namespace Users.API.Models.Shared;

public class UserResource : Resource<Users.API.Models.Response.v1.UserResponse>
{
    public IList<UserResource>? Embedded { get; private set; }

    public void AddEmbedded(IList<UserResource> embedded)
    {
        //if (embedded == null) return;
        if (Embedded == null) Embedded = new List<UserResource>();
        Embedded = embedded;

        //foreach (var resource in embedded)
        //{
        //    AddEmbedded(resource);
        //}
    }

    public override string ToString()
    {
        var data = "Empty";
        var numberOfLinks = 0;
        var numberOfEmbedded = 0;

        if (Data != null)
        {
            data = $"{Data.FirstName} {Data.LastName}";
        }

        if (Links != null)
        {
            numberOfLinks = Links.Count;
        }

        if (Embedded != null)
        {
            numberOfEmbedded = Embedded.Count;
        }

        return $"Data: {data} | Links: {numberOfLinks} | Embedded: {numberOfEmbedded}";
    }
}