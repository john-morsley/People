namespace Users.Application.Handlers;

public sealed class DeleteUserCommandHandler : IRequestHandler<Users.Application.Commands.DeleteUserCommand>
{
    private readonly Users.Domain.Interfaces.IUserRepository _userRepository;

    public DeleteUserCommandHandler(Users.Domain.Interfaces.IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Unit> Handle(Users.Application.Commands.DeleteUserCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        await _userRepository.DeleteAsync(command.Id);
        //var numberOfRowsAffected = await _userRepository.CompleteAsync();
        // ToDo --> Log!

        return Unit.Value;
    }
}
