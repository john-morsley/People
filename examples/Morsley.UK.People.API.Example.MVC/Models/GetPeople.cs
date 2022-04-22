namespace Morsley.UK.People.API.Example.MVC.Models;

public class GetPeople
{

    public int PageNumber { get; set; } = Defaults.DefaultPageNumber;

    public int PageSize { get; set; } = Defaults.DefaultPageSize;

    public string? Fields { get; set; }

    public string? Filter { get; set; }

    public string? Search { get; set; }

    public string? Sort { get; set; } = Defaults.DefaultPageSort;

    public bool NoResults { get; set; } = false;

    public PersonResource? Resource { get; set; }
}