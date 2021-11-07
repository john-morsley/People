using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Users.Application.Handlers
{
    public sealed class PartiallyUpdateUserCommandHandler : IRequestHandler<Users.Application.Commands.PartiallyUpdateUserCommand, Users.Domain.Models.User>
    {
        private readonly Users.Domain.Interfaces.IUnitOfWork _unitOfWork;

        public PartiallyUpdateUserCommandHandler(Users.Domain.Interfaces.IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Users.Domain.Models.User> Handle(Users.Application.Commands.PartiallyUpdateUserCommand command, CancellationToken ct)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var user = await _unitOfWork.UserRepository.Get(command.Id);
            if (user == null) throw new ArgumentException("User not found!", nameof(command));

            UpdateUserFromCommand(user, command);

            _unitOfWork.UserRepository.Update(user);
            var numberOfRowsAffected = await _unitOfWork.CompleteAsync();
            // ToDo --> Log!

            return user;
        }

        private void UpdateUserFromCommand(Users.Domain.Models.User user, Users.Application.Commands.PartiallyUpdateUserCommand command)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (command == null) throw new ArgumentNullException(nameof(command));

            if (command.TitleChanged && user.Title != command.Title) user.Title = command.Title;
            if (command.FirstNameChanged && user.FirstName != command.FirstName) user.FirstName = command.FirstName;
            if (command.LastNameChanged && user.LastName != command.LastName) user.LastName = command.LastName;
            if (command.SexChanged && user.Sex != command.Sex) user.Sex = command.Sex;
        }
    }
}