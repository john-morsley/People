namespace Morsley.UK.People.Domain.Extensions
{
    public static class DateTimeExtensions
    {
        public static int Age(this DateTime to, DateTime from)
        {
            var now = (from.Year * 100 + from.Month) * 100 + from.Day;
            var dob = (to.Year * 100 + to.Month) * 100 + to.Day;
            int age = (now - dob) / 10000;
            return age;
        }

        public static string InternationalFormat(this DateTime? dt) 
        {
            if (dt == null) return null;

            return $"{dt.Value.Year:0000}-{dt.Value.Month:00}-{dt.Value.Day:00}";
        }
    }
}