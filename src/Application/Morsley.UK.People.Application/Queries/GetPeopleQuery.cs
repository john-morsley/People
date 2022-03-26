namespace Morsley.UK.People.Application.Queries;

public sealed class GetPeopleQuery : IRequest<IPagedList<Person>>
{
    public uint PageNumber { get; set; }

    public uint PageSize { get; set; }

    public string? Search { get; set; }

    public string? Sort { get; set; }

    public string? Filter { get; set; }
}