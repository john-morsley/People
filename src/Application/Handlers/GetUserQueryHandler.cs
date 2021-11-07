using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Users.Application.Handlers
{
    public sealed class GetUserQueryHandler : IRequestHandler<Users.Application.Queries.GetUserQuery, Users.Domain.Models.User>
    {
        private readonly Users.Domain.Interfaces.IUnitOfWork _unitOfWork;

        public GetUserQueryHandler(Users.Domain.Interfaces.IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Users.Domain.Models.User> Handle(Users.Application.Queries.GetUserQuery query, CancellationToken ct)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var user = await _unitOfWork.UserRepository.Get(query.Id);
            return user;
        }
    }
}