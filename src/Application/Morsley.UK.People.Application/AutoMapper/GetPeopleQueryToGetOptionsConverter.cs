namespace Morsley.UK.People.Application.AutoMapper;

public class GetPeopleQueryToGetOptionsConverter : ITypeConverter<GetPeopleQuery, GetOptions>
{
    public GetOptions Convert(
        GetPeopleQuery source,
        GetOptions destination,
        ResolutionContext context)
    {
        var getOptions = new GetOptions();

        getOptions.PageNumber = source.PageNumber;
        getOptions.PageSize = source.PageSize;
        getOptions.Search = source.Search;

        if (!string.IsNullOrEmpty(source.Filter))
        {
            var filters = source.Filter.Split(',');
            foreach (var filter in filters) 
            {
                if (string.IsNullOrEmpty(filter)) throw new ArgumentException("Filter invalid. Found an empty filter section.", nameof(source));

                var parts = filter.Split(':');
                var field = parts[0];

                if (parts.Length == 1)
                {
                    if (string.IsNullOrEmpty(field)) throw new ArgumentException("Filter (field) invalid. Found an empty field section.", nameof(source));
                    getOptions.AddFilter(new Filter(field));
                }
                else if (parts.Length == 2)
                {
                    var value = parts[1];
                    if (string.IsNullOrEmpty(field)) throw new ArgumentException("Filter (field:value) invalid. Found an empty field section.", nameof(source));
                    getOptions.AddFilter(new Filter(field, value));
                }
            }
        }

        if (!string.IsNullOrEmpty(source.Sort))
        {
            var sorts = source.Sort.Split(',');
            foreach (var sort in sorts)
            {
                if (string.IsNullOrEmpty(sort)) throw new ArgumentException(nameof(source), "Sort (field:order,field:order) invalid. Found an empty field|order section.");

                var parts = sort.Split(':');
                if (parts.Length > 2) throw new ArgumentException(nameof(source), "Sort (field:order,field:order) invalid. Found more than 2 parts to a field:order section.");

                if (parts.Length == 1)
                {
                    var field = parts[0];
                    if (string.IsNullOrEmpty(field)) throw new ArgumentException(nameof(source), "Sort (field:order) invalid. field missing.");
                    var sortOrder = SortOrder.Ascending;
                    getOptions.AddOrdering(new Ordering(field, sortOrder));
                }
                else if (parts.Length == 2)
                {
                    var field = parts[0];
                    if (string.IsNullOrEmpty(field)) throw new ArgumentException(nameof(source), "Sort (field:order) invalid. field missing.");
                    var order = parts[1];
                    if (string.IsNullOrEmpty(order)) throw new ArgumentException(nameof(source), "Sort (field:order) invalid. Order missing.)");
                    var sortOrder = SortOrder.Ascending;
                    if (order.ToLower() == "desc" || order.ToLower() == "descending") sortOrder = SortOrder.Descending;
                    getOptions.AddOrdering(new Ordering(field, sortOrder));
                }
            }
        }

        return getOptions;
    }
}