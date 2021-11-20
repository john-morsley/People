namespace Users.Application.Handlers;

public sealed class UpdateUserCommandHandler : IRequestHandler<Users.Application.Commands.UpdateUserCommand, Users.Domain.Models.User>
{
    private readonly Users.Domain.Interfaces.IUserRepository _userRepository;

    public UpdateUserCommandHandler(Users.Domain.Interfaces.IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Users.Domain.Models.User> Handle(Users.Application.Commands.UpdateUserCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        Domain.Models.User user = await _userRepository.GetByIdAsync(command.Id);
        if (user == null) throw new ArgumentException("User not found!", nameof(command));
        return await UpdateUser(user, command, ct);
    }

    private async Task<Users.Domain.Models.User> UpdateUser(Users.Domain.Models.User user, Users.Application.Commands.UpdateUserCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        //if (user.Title != command.Title) user.Title = command.Title;
        if (user.FirstName != command.FirstName) user.FirstName = command.FirstName;
        if (user.LastName != command.LastName) user.LastName = command.LastName;
        if (user.Sex != command.Sex) user.Sex = command.Sex;

        await _userRepository.UpdateAsync(user);
        //var numberOfRowsAffected = await _userRepository.CompleteAsync();
        // ToDo --> Log!

        return user;
    }
}
