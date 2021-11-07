using System;
using System.Threading.Tasks;

namespace Users.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }

        Task<int> CompleteAsync();
    }
}