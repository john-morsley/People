using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;

namespace Users.Application.Handlers
{
    public sealed class GetPageOfUsersQueryHandler : IRequestHandler<Users.Application.Queries.GetPageOfUsersQuery, Users.Domain.Interfaces.IPagedList<Users.Domain.Models.User>>
    {
        private readonly Users.Domain.Interfaces.IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPageOfUsersQueryHandler(Users.Domain.Interfaces.IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Users.Domain.Interfaces.IPagedList<Users.Domain.Models.User>> Handle(Users.Application.Queries.GetPageOfUsersQuery query, CancellationToken ct)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var getOptions = _mapper.Map<Users.Application.Models.GetOptions>(query);
            var pageOfUsers = await _unitOfWork.UserRepository.Get(getOptions);
            return pageOfUsers;
        }
    }
}