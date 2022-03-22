using Microsoft.AspNetCore.Http;
using Morsley.UK.People.API.Contracts.Shared;
using System.Collections.Generic;
using System.Dynamic;

namespace Morsley.UK.People.API.Common.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected BaseController(IHttpContextAccessor context, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            Context = context;
            //ApiVersionDescriptionProvider = apiVersionDescriptionProvider;
            //var baseUrl = BaseUrl();
            //var request = context.HttpContext.Request;
            //var temp = FullUrl();
        }

        protected IConfiguration? Configuration { get; set; }

        public IHttpContextAccessor Context { get; }

        //public IApiVersionDescriptionProvider ApiVersionDescriptionProvider { get; }

        protected ExpandoObject? AddLinks(IDictionary<string, object> shapedPerson, Guid personId)
        {
            var links = CreateLinksForPerson(personId);
            shapedPerson.Add("_links", links);
            return shapedPerson as ExpandoObject;
        }

        protected IEnumerable<Link> CreateLinksForPerson(Guid personId)
        {
            var links = new List<Link>();

            //string url;
            //Link link;

            //if (string.IsNullOrWhiteSpace(getPersonRequest.Fields))
            //{
            //var getPersonLink = GetPersonLink(personId);
            //url = Url.Action("GetPerson", "Persons.API.Read.Controllers.v1.PersonsController", new { personId });
            //var url = Url.Link("GetPerson", new { personId });
            //link = new Link(url, "self", "GET");
            //links.Add(getPersonLink);
            //}
            //else
            //{
            var getPersonLink = GetPersonLink(personId);
            //url = Url.Action(nameof(Persons.API.Read.Controllers.v1.PersonsController.Get), nameof(Persons.API.Read.Controllers.v1.PersonsController), new { personId });
            //url = Url.Link("GetPerson", new { personId, getPersonRequest });
            //link = new Link(url, "self", "GET");
            links.Add(getPersonLink);
            //}

            //url = Url.Link("DeletePerson", new { personId });
            var deletePersonLink = DeletePersonLink(personId);
            links.Add(deletePersonLink);

            //url = Url.Link("CreatePerson", new { personId });
            //link = new Link(url, "create_person", "POST");
            //links.Add(link);

            return links;
        }

        protected virtual Link GetPersonLink(Guid personId)
        {
            var url = FullUrl();
            var getPersonUrl = $"{url}/{personId}";
            //var test = UrlHelperExtensions.("demo", values);
            //Uri test = new Uri(url);
        
            var link = new Link(getPersonUrl, "self", "GET");
            return link;        
        }

        protected virtual Link DeletePersonLink (Guid personId)
        {
            var url = FullUrl();
            var getPersonUrl = $"{url}/{personId}";
            var link = new Link(getPersonUrl, "self", "DELETE");
            return link;
        }

        private string BaseUrl()
        {
            var request = Context?.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";
            return baseUrl;
        }

        private string BaseUrlWithVersion()
        {
            var baseUrl = BaseUrl();

            //var version = ApiVersionDescriptionProvider.ApiVersionDescriptions.First().GroupName;

            return $"{baseUrl}/api"; ///{version}";
        }

        private string FullUrl()
        {
            //return $"{BaseUrlWithVersion()}/{ControllerName()}";
            return $"{ControllerName()}";
        }

        private string ControllerName()
        {
            var controllerValue = this.ControllerContext.RouteData.Values["controller"];
            if (controllerValue == null) throw new NullReferenceException();
            var controllerName = controllerValue.ToString();
            if (controllerName == null) throw new NullReferenceException();
            return controllerName.ToLower();
        }
    }
}
