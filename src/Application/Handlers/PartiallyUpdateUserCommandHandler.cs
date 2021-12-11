namespace Users.Application.Handlers;

public sealed class PartiallyUpdateUserCommandHandler : IRequestHandler<Users.Application.Commands.PartiallyUpdateUserCommand, Users.Domain.Models.User>
{
    private readonly Users.Domain.Interfaces.IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public PartiallyUpdateUserCommandHandler(Users.Domain.Interfaces.IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Users.Domain.Models.User> Handle(Users.Application.Commands.PartiallyUpdateUserCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        Domain.Models.User user = _mapper.Map<Domain.Models.User>(command);

        await _userRepository.UpdateAsync(user);
        //var numberOfRowsAffected = await _userRepository.CompleteAsync();
        // ToDo --> Log!

        return user;
    }
}
