namespace Morsley.UK.People.Persistence.Repositories
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository(IMongoContext context) : base(context, "people") { }

        protected override IQueryable<Person> Filter(IQueryable<Person> entities, IGetOptions options)
        {
            if (!options.Filters.Any()) return entities;

            var people = base.Filter(entities, options);

            var personFilters = ExtractPersonSpecificFilters(options.Filters);

            if (personFilters.Any())
            {
                foreach (var filter in personFilters)
                {
                    var filterPredicate = FilterPredicate(filter);
                    people = people.Where(filterPredicate);
                }
            }

            return people;
        }

        protected override IQueryable<Person> Search(IQueryable<Person> entities, IGetOptions options)
        {
            var people = base.Search(entities, options);

            if (string.IsNullOrWhiteSpace(options.Search)) return people;

            return people.Where(person => person.FirstName.Contains(options.Search) ||
                                          person.LastName.Contains(options.Search));
        }

        private IEnumerable<IFilter> ExtractPersonSpecificFilters(IEnumerable<IFilter> originalFilters)
        {
            var personFilters = new List<IFilter>();

            foreach (var filter in originalFilters)
            {
                if (IsFilterPersonSpecific(filter))
                {
                    personFilters.Add(filter);
                }
            }

            return personFilters;
        }

        private string FilterPredicate(IFilter filter)
        {
            if (filter.Key.Equals("Sex", StringComparison.CurrentCultureIgnoreCase) ||
                filter.Key.Equals("Gender", StringComparison.CurrentCultureIgnoreCase) ||
                filter.Key.Equals("FirstName", StringComparison.CurrentCultureIgnoreCase) ||
                filter.Key.Equals("LastName", StringComparison.CurrentCultureIgnoreCase) ||
                filter.Key.Equals("DateOfBirth", StringComparison.CurrentCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(filter.Value)) return $"{filter.Key} = null";
                return $"{filter.Key} = \"{filter.Value}\"";
            }

            return $"{filter.Key} = {filter.Value}";
        }

        private bool IsFilterPersonSpecific(IFilter filter)
        {
            return filter.Key.Equals("FirstName", StringComparison.CurrentCultureIgnoreCase) ||
                   filter.Key.Equals("LastName", StringComparison.CurrentCultureIgnoreCase) ||
                   filter.Key.Equals("Sex", StringComparison.CurrentCultureIgnoreCase) ||
                   filter.Key.Equals("Gender", StringComparison.CurrentCultureIgnoreCase) ||
                   filter.Key.Equals("DateOfBirth", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}