using Users.Domain.Interfaces;
using Users.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Users.Persistence.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        protected override IQueryable<User> Filter(IQueryable<User> entities, IGetOptions options)
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<User> Search(IQueryable<User> entities, IGetOptions options)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<IFilter> ExtractUserSpecificFilters(IEnumerable<IFilter> originalFilters)
        {
            throw new NotImplementedException();
        }
    }
}