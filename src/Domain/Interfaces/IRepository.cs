namespace Users.Domain.Interfaces;

public interface IRepository<TEntity> : IDisposable where TEntity : class
{
    Task<bool> ExistsAsync(Guid id);

    Task<TEntity> GetByIdAsync(Guid id);

    Task<IPagedList<TEntity>> GetPageAsync(IGetOptions options);

    Task AddAsync(TEntity obj);

    Task UpdateAsync(TEntity obj);

    Task DeleteAsync(Guid id);
}
