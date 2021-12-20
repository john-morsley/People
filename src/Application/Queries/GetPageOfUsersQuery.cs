namespace Users.Application.Queries;

public sealed class GetPageOfUsersQuery : IRequest<IPagedList<User>>
{
    public uint PageNumber { get; set; }

    public uint PageSize { get; set; }

    public string SearchQuery { get; set; }

    public string OrderBy { get; set; }
}
