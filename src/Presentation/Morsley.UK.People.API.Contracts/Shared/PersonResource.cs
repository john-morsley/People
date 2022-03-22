namespace Morsley.UK.People.API.Contracts.Shared
{
    public class PersonResource : Resource<PersonResponse>
    {
        public IList<PersonResource>? Embedded { get; private set; }

        public void AddEmbedded(IList<PersonResource> embedded)
        {
            //if (embedded == null) return;
            if (Embedded == null) Embedded = new List<PersonResource>();
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
}