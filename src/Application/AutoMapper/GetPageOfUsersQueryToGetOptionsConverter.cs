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
        getOptions.Search = source.Search;

        if (!string.IsNullOrEmpty(source.Filter))
        {
            var filters = source.Filter.Split(',');
            foreach (var filter in filters) 
            {
                if (string.IsNullOrEmpty(filter)) throw new ArgumentException(nameof(source), "Filter invalid. Found an empty filter section.");

                var parts = filter.Split(':');
                var field = parts[0];
                var value = parts[1];
                if (string.IsNullOrEmpty(field)) throw new ArgumentException(nameof(source), "Filter (field:value) invalid. Found an empty field section.");
                if (string.IsNullOrEmpty(value)) throw new ArgumentException(nameof(source), "Filter (field:value) invalid. Found an empty value section.");
                getOptions.AddFilter(new Filter(field, value));
            }
        }

        if (!string.IsNullOrEmpty(source.OrderBy))
        {
            var orderBys = source.OrderBy.Split(',');
            foreach (var orderBy in orderBys)
            {
                if (string.IsNullOrEmpty(orderBy)) throw new ArgumentException(nameof(source), "OrderBy (field:order,field:order) invalid. Found an empty field|order section.");

                var parts = orderBy.Split(':');
                if (parts.Length > 2) throw new ArgumentException(nameof(source), "OrderBy (field:order,field:order) invalid. Found more than 2 parts to a field:order section.");

                if (parts.Length == 1)
                {
                    var field = parts[0];
                    if (string.IsNullOrEmpty(field)) throw new ArgumentException(nameof(source), "OrderBy (field:order) invalid. field missing.");
                    var sortOrder = SortOrder.Ascending;
                    getOptions.AddOrdering(new Ordering(field, sortOrder));
                }
                else if (parts.Length == 2)
                {
                    var field = parts[0];
                    if (string.IsNullOrEmpty(field)) throw new ArgumentException(nameof(source), "OrderBy (field:order) invalid. field missing.");
                    var order = parts[1];
                    if (string.IsNullOrEmpty(order)) throw new ArgumentException(nameof(source), "OrderBy (field:order) invalid. Order missing.)");
                    var sortOrder = SortOrder.Ascending;
                    if (order.ToLower() == "desc" || order.ToLower() == "descending") sortOrder = SortOrder.Descending;
                    getOptions.AddOrdering(new Ordering(field, sortOrder));
                }
            }
        }

        return getOptions;
    }
}
