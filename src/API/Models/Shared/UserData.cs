using Users.API.Models.Response.v1;

namespace Users.API.Models.Shared
{
    public class UserData
    {
        public UserResponse User { get; set; }

        public IList<Link> Links { get; set; }

        public IList<UserData> Embedded { get; set; }

        public override string ToString()
        {
            var user = "Empty";
            var numberOfLinks = 0;
            var numberOfEmbedded = 0;

            if (User != null)
            {
                user = $"{User.FirstName} {User.LastName}";
            }

            if (Links != null)
            {
                numberOfLinks = Links.Count;
            }

            if (Embedded != null)
            {
                numberOfEmbedded = Embedded.Count;
            }

            return $"User: {user} | Links: {numberOfLinks} | Embedded: {numberOfEmbedded}";
        }
    }
}
