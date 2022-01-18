using Microsoft.AspNetCore.Http;

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

    protected IConfiguration Configuration { get; set; }

    public IHttpContextAccessor Context { get; }
    public IApiVersionDescriptionProvider ApiVersionDescriptionProvider { get; }

    protected virtual Users.API.Models.Shared.Link GetUserLink(Users.API.Models.Request.v1.GetUserRequest getUserRequest)
    {
        var url = FullUrl();
        var getUserUrl = $"{url}/{getUserRequest.Id}";
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
        var request = Context.HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
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
        return this.ControllerContext.RouteData.Values["controller"].ToString().ToLower();
        //return Request.RouteValues["controller"].ToString().ToLower();
    }
}
