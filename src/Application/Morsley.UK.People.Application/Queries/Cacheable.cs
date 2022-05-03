namespace Morsley.UK.People.Application.Queries;

public class Cacheable : ICacheable
{
    public string CacheKey
    {
        get
        {
            // ToDo --> Should equal parent Name + Id (ToString)
            return $"{this}";
        }
    }
}