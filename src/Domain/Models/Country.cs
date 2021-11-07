using System;
using Users.Domain.Interfaces;

namespace Users.Domain.Models
{
    public class Country : Entity<Guid>
    {
        public string Value { get; set; }

        public string TwoLetterCode { get; set; }

        public string ThreeLetterCode { get; set; }
    }
}