namespace Morsley.UK.People.Persistence.Interfaces;

public interface IMongoContext : IDisposable
{
    IMongoCollection<T> GetCollection<T>(string name);
}