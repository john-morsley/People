namespace Morsley.UK.People.Domain.Models
{
    public class Email
    {
        public string Value { get; set; }

        public bool IsValidated { get; set; }

        public bool IsPrimary { get; set; }
    }
}
