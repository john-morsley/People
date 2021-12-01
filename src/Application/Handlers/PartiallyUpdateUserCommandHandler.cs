namespace Users.Application.Handlers;

public sealed class PartiallyUpdateUserCommandHandler : IRequestHandler<Users.Application.Commands.PartiallyUpdateUserCommand, Users.Domain.Models.User>
{
    private readonly Users.Domain.Interfaces.IUserRepository _userRepository;

    public PartiallyUpdateUserCommandHandler(Users.Domain.Interfaces.IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Users.Domain.Models.User> Handle(Users.Application.Commands.PartiallyUpdateUserCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var user = await _userRepository.GetByIdAsync(command.Id);
        if (user == null) throw new ArgumentException("User not found!", nameof(command));

        UpdateUserFromCommand(user, command);

        await _userRepository.UpdateAsync(user);
        //var numberOfRowsAffected = await _userRepository.CompleteAsync();
        // ToDo --> Log!

        return user;
    }

    private void UpdateUserFromCommand(Users.Domain.Models.User user, Users.Application.Commands.PartiallyUpdateUserCommand command)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (command == null) throw new ArgumentNullException(nameof(command));

        //if (command.TitleChanged && user.Title != command.Title) user.Title = command.Title;
        if (command.FirstNameChanged && user.FirstName != command.FirstName) user.FirstName = command.FirstName;
        if (command.LastNameChanged && user.LastName != command.LastName) user.LastName = command.LastName;
        //if (command.SexChanged && user.Sex != command.Sex) user.Sex = command.Sex;
    }
}
