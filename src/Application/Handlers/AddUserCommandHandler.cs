﻿namespace Users.Application.Handlers;

public sealed class AddUserCommandHandler : IRequestHandler<Users.Application.Commands.AddUserCommand, Guid>
{
    private readonly Users.Domain.Interfaces.IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public AddUserCommandHandler(Users.Domain.Interfaces.IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Guid> Handle(Users.Application.Commands.AddUserCommand command, CancellationToken ct)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        Domain.Models.User user = _mapper.Map<Domain.Models.User>(command);

        await _userRepository.CreateAsync(user);
        //var numberOfRowsAffected = await _userRepository.CompleteAsync();
        // ToDo --> Log!

        return user.Id;
    }
}
