//namespace Users.API.Read.AutoMapper
//{
//    public class DateOfBirthTypeConverter : ITypeConverter<Domain.Models.DateOfBirth, string>
//    {
//        public string Convert(
//            Domain.Models.DateOfBirth source,
//            string destination,
//            ResolutionContext context)
//        {
//            if (source == null) return null;

//            string conversion = $"{source.Year:0000}-{source.Month:00}-{source.Day:00}";

//            return conversion;
//        }
//    }
//}
