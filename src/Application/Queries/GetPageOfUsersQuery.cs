namespace Users.Application.Queries;

public sealed class GetPageOfUsersQuery : IRequest<IPagedList<User>>
{
    public uint PageNumber { get; set; }

    public uint PageSize { get; set; }

    public string? Search { get; set; }

    public string? Sort { get; set; }

    public string? Filter { get; set; }
}
