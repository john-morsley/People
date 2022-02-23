namespace Users.Application.Handlers;

public sealed class AddUserCommandHandler : IRequestHandler<Users.Application.Commands.AddUserCommand, Users.Domain.Models.User>
{
    private readonly Users.Domain.Interfaces.IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public AddUserCommandHandler(Users.Domain.Interfaces.IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Users.Domain.Models.User> Handle(Users.Application.Commands.AddUserCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var user = _mapper.Map<Domain.Models.User>(command);

        await _userRepository.AddAsync(user);

        return user;
    }
}
