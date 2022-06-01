namespace Morsley.UK.People.Application.Queries;

public abstract class Cacheable : ICacheable
{
    public abstract string CacheKey { get; }

    public bool DoNotCache { get; set; } = false;

    public TimeSpan TimeToLive { get; set; } = TimeSpan.FromMinutes(1);
}