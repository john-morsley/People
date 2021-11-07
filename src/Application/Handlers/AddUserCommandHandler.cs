using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;

namespace Users.Application.Handlers
{
    public sealed class AddUserCommandHandler : IRequestHandler<Users.Application.Commands.AddUserCommand, Guid>
    {
        private readonly Users.Domain.Interfaces.IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddUserCommandHandler(Users.Domain.Interfaces.IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Guid> Handle(Users.Application.Commands.AddUserCommand command, CancellationToken ct)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            Domain.Models.User user = _mapper.Map<Domain.Models.User>(command);

            _unitOfWork.UserRepository.Create(user);
            var numberOfRowsAffected = await _unitOfWork.CompleteAsync();
            // ToDo --> Log!

            return user.Id;
        }
    }
}