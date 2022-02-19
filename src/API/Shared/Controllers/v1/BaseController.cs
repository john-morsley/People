using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Dynamic;
using Users.API.Models.Shared;

namespace Users.API.Shared.Controllers.v1;

public abstract class BaseController : ControllerBase
{
    public BaseController(IHttpContextAccessor context, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        Context = context;
        ApiVersionDescriptionProvider = apiVersionDescriptionProvider;
        //var baseUrl = BaseUrl();
        //var request = context.HttpContext.Request;
        //var temp = FullUrl();
    }

    protected IConfiguration? Configuration { get; set; }

    public IHttpContextAccessor Context { get; }

    public IApiVersionDescriptionProvider ApiVersionDescriptionProvider { get; }

    protected ExpandoObject? AddLinks(IDictionary<string, object> shapedUser, Guid userId)
    {
        var links = CreateLinksForUser(userId);
        shapedUser.Add("_links", links);
        return shapedUser as ExpandoObject;
    }

    protected IEnumerable<Link> CreateLinksForUser(Guid userId)
    {
        var links = new List<Link>();

        //string url;
        //Link link;

        //if (string.IsNullOrWhiteSpace(getUserRequest.Fields))
        //{
        //var getUserLink = GetUserLink(userId);
        //url = Url.Action("GetUser", "Users.API.Read.Controllers.v1.UsersController", new { userId });
        //var url = Url.Link("GetUser", new { userId });
        //link = new Link(url, "self", "GET");
        //links.Add(getUserLink);
        //}
        //else
        //{
        var getUserLink = GetUserLink(userId);
        //url = Url.Action(nameof(Users.API.Read.Controllers.v1.UsersController.Get), nameof(Users.API.Read.Controllers.v1.UsersController), new { userId });
        //url = Url.Link("GetUser", new { userId, getUserRequest });
        //link = new Link(url, "self", "GET");
        links.Add(getUserLink);
        //}

        //url = Url.Link("DeleteUser", new { userId });
        var deleteUserLink = DeleteUserLink(userId);
        links.Add(deleteUserLink);

        //url = Url.Link("CreateUser", new { userId });
        //link = new Link(url, "create_user", "POST");
        //links.Add(link);

        return links;
    }


    protected virtual Users.API.Models.Shared.Link GetUserLink(Guid userId)
    {
        var url = FullUrl();
        var getUserUrl = $"{url}/{userId}";
        //var test = UrlHelperExtensions.("demo", values);
        //Uri test = new Uri(url);
        
        var link = new Users.API.Models.Shared.Link(getUserUrl, "self", "GET");
        return link;        
    }

    protected virtual Users.API.Models.Shared.Link DeleteUserLink (Guid userId)
    {
        var url = FullUrl();
        var getUserUrl = $"{url}/{userId}";
        var link = new Users.API.Models.Shared.Link(getUserUrl, "self", "DELETE");
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

        var version = ApiVersionDescriptionProvider.ApiVersionDescriptions.First().GroupName;

        return $"{baseUrl}/api/{version}";
    }

    private string FullUrl()
    {        
        return $"{BaseUrlWithVersion()}/{ControllerName()}";
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
