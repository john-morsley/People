using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Users.Application.Handlers
{
    public sealed class UserExistsQueryHandler : IRequestHandler<Users.Application.Queries.UserExistsQuery, bool>
    {
        private readonly Users.Domain.Interfaces.IUnitOfWork _unitOfWork;

        public UserExistsQueryHandler(Users.Domain.Interfaces.IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(Users.Application.Queries.UserExistsQuery query, CancellationToken ct)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            return await _unitOfWork.UserRepository.Exists(query.Id);
        }
    }
}