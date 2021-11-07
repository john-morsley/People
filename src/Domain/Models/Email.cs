using System;
using Users.Domain.Interfaces;

namespace Users.Domain.Models
{
    public class Email : Entity<Guid>
    {
        public string Value { get; set; }

        public bool IsValidated { get; set; }

        public bool IsPrimary { get; set; }
    }
}