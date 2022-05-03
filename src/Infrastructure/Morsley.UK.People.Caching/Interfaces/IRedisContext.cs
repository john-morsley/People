namespace Morsley.UK.People.Caching.Interfaces;

public interface IRedisContext
{
    IDatabase GetDatabase();
}