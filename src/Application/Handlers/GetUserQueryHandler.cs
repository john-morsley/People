namespace Users.Application.Handlers;

public sealed class GetUserQueryHandler : IRequestHandler<Users.Application.Queries.GetUserQuery, Users.Domain.Models.User>
{
    private readonly Users.Domain.Interfaces.IUserRepository _userRepository;

    public GetUserQueryHandler(Users.Domain.Interfaces.IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Users.Domain.Models.User> Handle(Users.Application.Queries.GetUserQuery query, CancellationToken ct)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        var user = await _userRepository.GetByIdAsync(query.Id);
        return user;
    }
}
