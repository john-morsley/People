using Morsley.UK.People.API.Contracts.Requests;
//using Morsley.UK.People.API.Contracts.Requests;
using Morsley.UK.People.API.Contracts.Responses;
using Morsley.UK.People.API.Contracts.Shared;
using Morsley.UK.People.Application.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using PersonResponse = Morsley.UK.People.API.Contracts.Responses.PersonResponse;

namespace Morsley.UK.People.API.Common.Helpers;
public class LinksHelper
{
    public static ExpandoObject? AddLinks(IDictionary<string, object> shapedPerson, Guid personId)
    {
        var links = CreateLinksForPerson(personId);
        shapedPerson.Add("_links", links);
        return shapedPerson as ExpandoObject;
    }

    public static IEnumerable<ExpandoObject> AddLinks(IEnumerable<ExpandoObject> shapedPageOfPersons)
    {
        foreach (var shapedPerson in shapedPageOfPersons)
        {
            if (shapedPerson is IDictionary<string, object> person)
            {
                var personId = (Guid)person["Id"];
                var personLinks = CreateLinksForPerson(personId);
                person.Add("_links", personLinks);
            }
        }

        return shapedPageOfPersons;
    }

    protected static IEnumerable<Link> CreateLinksForPerson(Guid personId)
    {
        var links = new List<Link>();

        var getPersonLink = GetPersonLink(personId);
        links.Add(getPersonLink);

        var updatePersonLink = UpdatePersonLink(personId);
        links.Add(updatePersonLink);

        var deletePersonLink = DeletePersonLink(personId);
        links.Add(deletePersonLink);


        return links;
    }

    public static IEnumerable<Link> CreateLinksForPageOfPersons(
        GetPeopleRequest getPageOfPersonsRequest,
        Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse> pageOfPersons)
    {
        var links = new List<Link>();

        // Previous page...
        if (pageOfPersons.HasPrevious)
        {
            var previousUrl = CreatePeopleResourceUri(getPageOfPersonsRequest, ResourceUriType.PreviousPage);
            if (previousUrl != null)
            {
                var previousLink = new Link(previousUrl, "previous", "GET");
                links.Add(previousLink);
            }
        }

        // Current page...
        var currentUrl = CreatePeopleResourceUri(getPageOfPersonsRequest, ResourceUriType.Current);
        if (currentUrl != null)
        {
            var currentLink = new Link(currentUrl, "self", "GET");
            links.Add(currentLink);
        }

        // Next page...
        if (pageOfPersons.HasNext)
        {
            var nextUrl = CreatePeopleResourceUri(getPageOfPersonsRequest, ResourceUriType.NextPage);
            if (nextUrl != null)
            {
                var nextLink = new Link(nextUrl, "next", "GET");
                links.Add(nextLink);
            }
        }

        return links;
    }

    private static string? CreatePeopleResourceUri(GetPeopleRequest getPageOfPersonsRequest, ResourceUriType type)
    {
        string? link;
        switch (type)
        {
            case ResourceUriType.PreviousPage:
                link = GetPeopleResourceUri(
                    getPageOfPersonsRequest.PageNumber - 1,
                    getPageOfPersonsRequest.PageSize,
                    getPageOfPersonsRequest.Fields,
                    getPageOfPersonsRequest.Filter,
                    getPageOfPersonsRequest.Search,
                    getPageOfPersonsRequest.Sort
                );
                return link;
            case ResourceUriType.NextPage:
                link = GetPeopleResourceUri(
                    getPageOfPersonsRequest.PageNumber + 1,
                    getPageOfPersonsRequest.PageSize,
                    getPageOfPersonsRequest.Fields,
                    getPageOfPersonsRequest.Filter,
                    getPageOfPersonsRequest.Search,
                    getPageOfPersonsRequest.Sort
                );
                return link;
            case ResourceUriType.Current:
                link = GetPeopleResourceUri(
                    getPageOfPersonsRequest.PageNumber,
                    getPageOfPersonsRequest.PageSize,
                    getPageOfPersonsRequest.Fields,
                    getPageOfPersonsRequest.Filter,
                    getPageOfPersonsRequest.Search,
                    getPageOfPersonsRequest.Sort
                );
                return link;
            default: throw new NotImplementedException();
        }
    }

    protected static Link DeletePersonLink(Guid personId)
    {
        var getPersonUrl = $"/api/person/{personId}";
        var link = new Link(getPersonUrl, "self", "DELETE");
        return link;
    }

    private static string GetPeopleResourceUri(
        int pageNumber,
        int pageSize,
        string? fields,
        string? filter,
        string? search,
        string? sort)
    {
        return $"/api/people?pageNumber={pageNumber}&pageSize={pageSize}&fields={fields}&filter={filter}&search={search}&sort={sort}";
    }

    protected static Link GetPersonLink(Guid personId)
    {
        var getPersonUrl = $"/api/person/{personId}";
        var link = new Link(getPersonUrl, "self", "GET");
        return link;
    }

    protected static Link UpdatePersonLink(Guid personId)
    {
        var updatePersonUrl = $"/api/person/{personId}";
        var link = new Link(updatePersonUrl, "self", "PUT");
        return link;
    }
}
