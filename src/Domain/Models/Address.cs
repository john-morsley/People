using System;
using Users.Domain.Interfaces;
using Users.Domain.Models;

namespace Users.Domain.Models
{
    public class Address : Entity<Guid>
    {
        //public IList<string> Lines { get; set; }

        public string County { get; set; }

        public string PostCode { get; set; }

        public Country Country { get; set; }

        public bool IsPrimary { get; set; }
    }
}