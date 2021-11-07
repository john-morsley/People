using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Users.Application.Handlers
{
    public sealed class DeleteUserCommandHandler : IRequestHandler<Users.Application.Commands.DeleteUserCommand>
    {
        private readonly Users.Domain.Interfaces.IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(Users.Domain.Interfaces.IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Unit> Handle(Users.Application.Commands.DeleteUserCommand command, CancellationToken ct)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            _unitOfWork.UserRepository.Delete(command.Id);
            var numberOfRowsAffected = await _unitOfWork.CompleteAsync();
            // ToDo --> Log!

            return Unit.Value;
        }
    }
}