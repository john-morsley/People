namespace Users.API.Read.AutoMapper
{
    public class NullableDateTimeTypeConverter : ITypeConverter<DateTime?, string>
    {
        public string Convert(
            DateTime? source,
            string destination,
            ResolutionContext context)
        {
            if (source == null) return null;

            string conversion = $"{source.Value.Year:0000}-{source.Value.Month:00}-{source.Value.Day:00}";

            return conversion;
        }
    }
}
