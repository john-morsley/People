namespace Users.Application.AutoMapper;

public class GetPageOfUsersQueryToGetOptionsConverter : ITypeConverter<Users.Application.Queries.GetPageOfUsersQuery, 
                                                                       Users.Application.Models.GetOptions>
{
    public Users.Application.Models.GetOptions Convert(
        Users.Application.Queries.GetPageOfUsersQuery source, 
        Users.Application.Models.GetOptions destination, 
        ResolutionContext context)
    {
        var getOptions = new Users.Application.Models.GetOptions();

        getOptions.PageNumber = source.PageNumber;
        getOptions.PageSize = source.PageSize;
        getOptions.SearchQuery = source.SearchQuery;

        if (string.IsNullOrEmpty(source.OrderBy)) return getOptions;

        var orderBys = source.OrderBy.Split(',');
        foreach (var orderBy in orderBys)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                throw new ArgumentException(nameof(source), "OrderBy (key|order,key|order) invalid. Found an empty key|order section.");
            }

            var parts = orderBy.Split('|');
            if (parts.Length > 2)
            {
                throw new ArgumentException(nameof(source), "OrderBy (key|order,key|order) invalid. Found more than 2 parts to a key|order section.");
            }

            if (parts.Length == 1)
            {
                var key = parts[0];
                if (string.IsNullOrEmpty(key)) throw new ArgumentException(nameof(source), "OrderBy (key|order) invalid. Key missing.");
                var sortOrder = SortOrder.Ascending;
                var ordering = new Ordering(key, sortOrder);
                getOptions.AddOrdering(ordering);

            }
            else if (parts.Length == 2)
            {
                var key = parts[0];
                if (string.IsNullOrEmpty(key)) throw new ArgumentException(nameof(source), "OrderBy (key|order) invalid. Key missing.");
                var order = parts[1];
                if (string.IsNullOrEmpty(order)) throw new ArgumentException(nameof(source), "OrderBy (key|order) invalid. Order missing.)");
                var sortOrder = SortOrder.Ascending;
                if (order.ToLower() == "desc" || order.ToLower() == "descending") sortOrder = SortOrder.Descending;
                var ordering = new Ordering(key, sortOrder);
                getOptions.AddOrdering(ordering);
            }
        }

        return getOptions;
    }
}
