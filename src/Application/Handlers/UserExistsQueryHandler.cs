namespace Users.Application.Handlers;

public sealed class UserExistsQueryHandler : IRequestHandler<Users.Application.Queries.UserExistsQuery, bool>
{
    private readonly Users.Domain.Interfaces.IUserRepository _userRepository;

    public UserExistsQueryHandler(Users.Domain.Interfaces.IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(Users.Application.Queries.UserExistsQuery query, CancellationToken ct)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        return await _userRepository.ExistsAsync(query.Id);
    }
}
