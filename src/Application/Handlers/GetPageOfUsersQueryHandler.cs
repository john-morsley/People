namespace Users.Application.Handlers;

public sealed class GetPageOfUsersQueryHandler : IRequestHandler<Users.Application.Queries.GetPageOfUsersQuery, Users.Domain.Interfaces.IPagedList<Users.Domain.Models.User>>
{
    private readonly Users.Domain.Interfaces.IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetPageOfUsersQueryHandler(Users.Domain.Interfaces.IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Users.Domain.Interfaces.IPagedList<Users.Domain.Models.User>> Handle(Users.Application.Queries.GetPageOfUsersQuery query, CancellationToken ct)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        var getOptions = _mapper.Map<Users.Application.Models.GetOptions>(query);
        var pageOfUsers = await _userRepository.GetPageAsync(getOptions);

        return pageOfUsers;
    }
}
