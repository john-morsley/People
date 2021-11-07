using System;

namespace Users.Application.Models
{
    public class Ordering : Users.Domain.Interfaces.IOrdering
    {
        public Ordering(string key, Users.Domain.Enumerations.SortOrder order)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key cannot be null or empty!", nameof(key));

            Key = key;
            Order = order;
        }

        public string Key { get; }

        public Users.Domain.Enumerations.SortOrder Order { get; }
    }
}