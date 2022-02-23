namespace Users.Persistence.Repositories;

public class UserRepository : Repository<Users.Domain.Models.User>, IUserRepository
{
    public UserRepository(IMongoContext context) : base(context, "users") { }

    protected override IQueryable<Users.Domain.Models.User> Filter(IQueryable<Users.Domain.Models.User> entities, IGetOptions options)
    {
        if (options == null) return entities;
        if (options.Filters == null) return entities;
        if (!options.Filters.Any()) return entities;

        var users = base.Filter(entities, options);

        var userFilters = ExtractUserSpecificFilters(options.Filters);

        if (userFilters.Any())
        {
            foreach (var filter in userFilters)
            {
                var filterPredicate = FilterPredicate(filter);
                users = users.Where(filterPredicate);
            }
        }

        return users;
    }

    protected override IQueryable<Users.Domain.Models.User> Search(IQueryable<Users.Domain.Models.User> entities, IGetOptions options)
    {
        var users = base.Search(entities, options);

        if (string.IsNullOrWhiteSpace(options.Search)) return users;

        return users.Where(u => u.FirstName.Contains(options.Search) ||
                                    u.LastName.Contains(options.Search));
    }

    private IEnumerable<IFilter> ExtractUserSpecificFilters(IEnumerable<IFilter> originalFilters)
    {
        var userFilters = new List<IFilter>();

        foreach (var filter in originalFilters)
        {
            if (IsFilterUserSpecific(filter))
            {
                userFilters.Add(filter);
            }
        }

        return userFilters;
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

    private bool IsFilterUserSpecific(IFilter filter)
    {
        return filter.Key.Equals("FirstName", StringComparison.CurrentCultureIgnoreCase) ||
               filter.Key.Equals("LastName", StringComparison.CurrentCultureIgnoreCase) ||
               filter.Key.Equals("Sex", StringComparison.CurrentCultureIgnoreCase) ||
               filter.Key.Equals("Gender", StringComparison.CurrentCultureIgnoreCase) ||
               filter.Key.Equals("DateOfBirth", StringComparison.CurrentCultureIgnoreCase);
    }
}