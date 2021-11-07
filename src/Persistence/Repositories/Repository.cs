using Users.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Users.Persistence.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity<Guid>
    {
        public void Create(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Exists(Guid id)
        {
            throw new NotImplementedException();
        }

        protected virtual IQueryable<TEntity> Filter(IQueryable<TEntity> entities, IGetOptions options)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Find(Func<TEntity, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IPagedList<TEntity>> Get(IGetOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var entities = GetAll(options);

            throw new NotImplementedException();
        }

        protected virtual IQueryable<TEntity> GetAll(IGetOptions options)
        {
            throw new NotImplementedException();
        }

        protected virtual IQueryable<TEntity> Search(IQueryable<TEntity> entities, IGetOptions options)
        {
            throw new NotImplementedException();
        }

        protected virtual IQueryable<TEntity> Sort(IQueryable<TEntity> entities, IGetOptions options)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}