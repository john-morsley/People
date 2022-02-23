using System;
using Users.Domain.Interfaces;

namespace Users.Domain.Models
{
    public class Phone : Entity<Guid>
    {
        public Phone() : base(Guid.NewGuid()) { }

        public string Value { get; set; }

        public bool IsValidated { get; set; }

        public bool IsPrimary { get; set; }
    }
}