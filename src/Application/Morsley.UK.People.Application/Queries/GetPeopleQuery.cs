﻿namespace Morsley.UK.People.Application.Queries;

public sealed class GetPeopleQuery : ICacheable, IRequest<PagedList<Person>>
{
    public uint PageNumber { get; set; }

    public uint PageSize { get; set; }

    public string? Search { get; set; }

    public string? Sort { get; set; }

    public string? Filter { get; set; }

    public override string ToString()
    {
        return $"PageNumber:{PageNumber}|" +
               $"PageSize:{PageSize}|" +
               $"Search:{Search.GetDisplayValue()}|" +
               $"Sort:{Sort.GetDisplayValue()}|" +
               $"Filter:{Filter.GetDisplayValue()}";
    }

    public string CacheKey => ToString();
}