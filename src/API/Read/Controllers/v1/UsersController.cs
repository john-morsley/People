namespace Users.API.Read.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class UsersController : Users.API.Shared.Controllers.v1.BaseController
{
    private readonly ILogger<UsersController> _logger;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UsersController(
        ILogger<UsersController> logger,
        IMediator mediator,
        IMapper mapper,
        IConfiguration configuration,
        IHttpContextAccessor context,
        IApiVersionDescriptionProvider apiVersionDescriptionProvider) : base(context, apiVersionDescriptionProvider)
    {
        _logger = logger;
        _mediator = mediator;
        _mapper = mapper;

        Configuration = configuration;
    }

    /// <summary>
    /// Get a single user by their unique identifier
    /// </summary>
    /// <param name="getUserRequest">
    /// The unique identifier for the user
    /// </param>
    /// <returns>The requested user (shaped, if required)</returns>
    /// <response code="200">Success - OK - Returns the requested user</response>
    /// <response code="204">Success - No Content - No user matched the given identifier</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    /// <response code="422">Error - Unprocessable Entity - The GetUserRequest object contained invalid data</response>
    // -------------------------------------------------- GET --> USER
    [HttpGet("{userId:guid}", Name = "GetUser")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Users.API.Models.Response.v1.UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Get(Users.API.Models.Request.v1.GetUserRequest getUserRequest)
    {
        if (getUserRequest == null) return BadRequest();
        if (getUserRequest.Id == Guid.Empty) return BadRequest();

        var userResponse = await GetUserResponse(getUserRequest);

        if (userResponse == null) return NoContent();

        var shapedUser = userResponse.ShapeData(getUserRequest.Fields);

        var shapedUserWithLinks = AddLinks(shapedUser, userResponse.Id);

        return Ok(shapedUserWithLinks);
    }

    /// <summary>
    /// Gets the content length of a user
    /// </summary>
    /// <param name="getUserRequest">
    /// A GetUserRequest object which contains fields for paging, searching, filtering, sorting and shaping user data</param>
    /// <returns>The content length of a page of users</returns>
    /// <response code="200">Success - OK - Returns the requested page of users</response>
    /// <response code="204">Success - No Content - No users matched given criteria</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    // -------------------------------------------------- HEAD --> USER
    [HttpHead("{userId:guid}", Name = "GetHead")]
    [ProducesResponseType(typeof(Users.API.Models.Response.v1.UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<Users.API.Models.Response.v1.UserResponse>> Head(Users.API.Models.Request.v1.GetUserRequest getUserRequest)
    {
        if (getUserRequest == null) return BadRequest();

        var userResponse = await GetUserResponse(getUserRequest);

        if (userResponse == null) return NoContent();

        var shapedUser = userResponse.ShapeData(getUserRequest.Fields);
        var linkedShapedUser = AddLinks(shapedUser, userResponse.Id);

        Response.ContentLength = CalculateContentLength(linkedShapedUser);

        return Ok();
    }

    /// <summary>
    /// Get a page of users
    /// </summary>
    /// <param name="getPageOfUsersRequest">
    /// A GetPageOfUsersRequest object contains fields for paging, searching, filtering, sorting and shaping user data</param>
    /// <returns>A page of users</returns>
    /// <response code="200">Success - OK - Returns the requested page of users</response>
    /// <response code="204">Success - No Content - No users matched given criteria</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    // -------------------------------------------------- GET --> PAGE OF USERS
    [HttpGet(Name = "GetUsers")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<Users.API.Models.Response.v1.UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<IEnumerable<Users.API.Models.Response.v1.UserResponse>>> Get(
        [FromQuery] Users.API.Models.Request.v1.GetPageOfUsersRequest getPageOfUsersRequest)
    {
        if (getPageOfUsersRequest == null) return BadRequest();

        var pageOfUserResponses = await GetPageOfUserResponses(getPageOfUsersRequest);

        if (!pageOfUserResponses.Any()) return NoContent();
        
        // Shape Users...
        var shapedPageOfUsers = GetUserResponses(pageOfUserResponses).ShapeData(getPageOfUsersRequest.Fields);

        // Add Metadata links...         
        var shapedPageOfUsersWithLinks = AddLinks(shapedPageOfUsers);
        var pageOfUsersLinks = CreateLinksForPageOfUsers(getPageOfUsersRequest, pageOfUserResponses);

        var shapedPageOfUsersWithLinksWithSelfLink = new ExpandoObject() as IDictionary<string, object>;
        shapedPageOfUsersWithLinksWithSelfLink.Add("_embedded", shapedPageOfUsersWithLinks);
        shapedPageOfUsersWithLinksWithSelfLink.Add("_links", pageOfUsersLinks);

        return Ok(shapedPageOfUsersWithLinksWithSelfLink);
    }

    private IEnumerable<ExpandoObject> AddLinks(IEnumerable<ExpandoObject> shapedPageOfUsers)
    {        
        foreach (var shapedUser in shapedPageOfUsers)
        {
            var user = shapedUser as IDictionary<string, object>;
            if (user != null)
            {
                var userId = (Guid)user["Id"];
                var userlinks = CreateLinksForUser(userId);
                user.Add("_links", userlinks);
            }
        }        

        return shapedPageOfUsers;
    }

    /// <summary>
    /// Gets the content length of a page of users
    /// </summary>
    /// <param name="getUsersRequest">
    /// A GetUsersRequest object which contains fields for paging, searching, filtering, sorting and shaping user data</param>
    /// <returns>The content length of a page of users</returns>
    /// <response code="200">Success - OK - Returns the requested page of users</response>
    /// <response code="204">Success - No Content - No users matched given criteria</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    // -------------------------------------------------- HEAD --> PAGE OF USERS
    [HttpHead]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<Users.API.Models.Response.v1.UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Head(
        [FromQuery] Users.API.Models.Request.v1.GetPageOfUsersRequest getPageOfUsersRequest)
    {
        if (getPageOfUsersRequest == null) return BadRequest();

        var pageOfUserResponses = await GetPageOfUserResponses(getPageOfUsersRequest);

        if (!pageOfUserResponses.Any()) return NoContent();

        // Shape Users...
        var shapedPageOfUsers = GetUserResponses(pageOfUserResponses).ShapeData(getPageOfUsersRequest.Fields);

        // Add Metadata links...         
        var shapedPageOfUsersWithLinks = AddLinks(shapedPageOfUsers);
        var pageOfUsersLinks = CreateLinksForPageOfUsers(getPageOfUsersRequest, pageOfUserResponses);

        var shapedPageOfUsersWithLinksWithSelfLink = new ExpandoObject() as IDictionary<string, object>;
        shapedPageOfUsersWithLinksWithSelfLink.Add("_embedded", shapedPageOfUsersWithLinks);
        shapedPageOfUsersWithLinksWithSelfLink.Add("_links", pageOfUsersLinks);

        Response.ContentLength = CalculateContentLength(shapedPageOfUsersWithLinksWithSelfLink);

        return Ok();
    }

    // -------------------------------------------------- OPTIONS --> ALLOWED HTTP METHODS
    [HttpOptions]
    public IActionResult GetOptions()
    {
        Response.Headers.Add("Allow", "GET,HEAD,OPTIONS");
        return Ok();
    }

    private long CalculateContentLength<T>(T obj) where T : class
    {
        var encoderSettings = new TextEncoderSettings();
        //encoderSettings.AllowCharacters('\u0026'); // &
        encoderSettings.AllowRange(UnicodeRanges.All);
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(encoderSettings),
            WriteIndented = false,
            Converters = { new JsonStringEnumConverter() }
        };

        // ToDo --> This feels like a hack. Needs further investigation.
        var json = System.Text.Json.JsonSerializer.Serialize(obj, options);

        json = System.Text.RegularExpressions.Regex.Unescape(json);

        return json.Length;
    }

    //private long CalculateContentLength(IEnumerable<Models.Response.v1.UserResponse> pageOfUserResponses)
    //{
    //    var options = new System.Text.Json.JsonSerializerOptions
    //    {
    //        Converters = { new Users.API.Models.Shared.PagedListJsonConverter() }
    //    };
    //    var json = System.Text.Json.JsonSerializer.Serialize(pageOfUserResponses, options);
    //    return json.Length;
    //}

    private IEnumerable<Link> CreateLinksForPageOfUsers(
        Users.API.Models.Request.v1.GetPageOfUsersRequest getPageOfUsersRequest,
        Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse> pageOfUsers)
    {
        var links = new List<Link>();

        // Previous page...
        if (pageOfUsers.HasPrevious)
        {
            var previousUrl = CreateUsersResourceUri(getPageOfUsersRequest, ResourceUriType.PreviousPage);
            var previousLink = new Link(previousUrl, "previous", "GET");
            links.Add(previousLink);
        }

        // Current page...
        var currentUrl = CreateUsersResourceUri(getPageOfUsersRequest, ResourceUriType.Current);
        var currentLink = new Link(currentUrl, "self", "GET");
        links.Add(currentLink);

        // Next page...
        if (pageOfUsers.HasNext)
        {
            var nextUrl = CreateUsersResourceUri(getPageOfUsersRequest, ResourceUriType.NextPage);
            var nextLink = new Link(nextUrl, "next", "GET");
            links.Add(nextLink);
        }

        return links;
    }

    private string CreateUsersResourceUri(Users.API.Models.Request.v1.GetPageOfUsersRequest getPageOfUsersRequest, Users.API.Models.Shared.ResourceUriType type)
    {
        string link;
        switch (type)
        {
            case Users.API.Models.Shared.ResourceUriType.PreviousPage:                
                link = Url.Link(
                    "GetUsers", 
                    new 
                    { 
                        pageNumber = getPageOfUsersRequest.PageNumber - 1, 
                        pageSize = getPageOfUsersRequest.PageSize,
                        fields = getPageOfUsersRequest.Fields,
                        filter = getPageOfUsersRequest.Filter,
                        search = getPageOfUsersRequest.Search,
                        sort = getPageOfUsersRequest.Sort
                    });  
                return link;
            case Users.API.Models.Shared.ResourceUriType.NextPage:
               link = Url.Link(
                    "GetUsers",
                    new
                    {
                        pageNumber = getPageOfUsersRequest.PageNumber + 1,
                        pageSize = getPageOfUsersRequest.PageSize,
                        fields = getPageOfUsersRequest.Fields,
                        filter = getPageOfUsersRequest.Filter,
                        search = getPageOfUsersRequest.Search,
                        sort = getPageOfUsersRequest.Sort
                    });
                return link;
            case Users.API.Models.Shared.ResourceUriType.Current:            
                link = Url.Link(
                    "GetUsers",
                    new
                    {
                        pageNumber = getPageOfUsersRequest.PageNumber,
                        pageSize = getPageOfUsersRequest.PageSize,
                        fields = getPageOfUsersRequest.Fields,
                        filter = getPageOfUsersRequest.Filter,
                        search = getPageOfUsersRequest.Search,
                        sort = getPageOfUsersRequest.Sort
                    });
                return link;
            default: throw new NotImplementedException();
        }
    }

    private async Task<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>> GetPageOfUserResponses(Users.API.Models.Request.v1.GetPageOfUsersRequest getPageOfUsersRequest)
    {
        if (getPageOfUsersRequest == null) throw new ArgumentNullException(nameof(getPageOfUsersRequest));

        var getPageOfUsersQuery = _mapper.Map<GetPageOfUsersQuery>(getPageOfUsersRequest);

        var pageOfUsers = await _mediator.Send(getPageOfUsersQuery);

        var pageOfUserResponses = _mapper.Map<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>(pageOfUsers);

        return pageOfUserResponses;
    }

    private Users.API.Models.Shared.Pagination GetPagination(
        Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse> pageOfUserResponses,
        Users.API.Models.Request.v1.GetPageOfUsersRequest getPageOfUsersRequest)
    {
        var previousPageLink = pageOfUserResponses.HasPrevious ? CreateUsersResourceUri(getPageOfUsersRequest, Users.API.Models.Shared.ResourceUriType.PreviousPage) : null;
        var nextPageLink = pageOfUserResponses.HasNext ? CreateUsersResourceUri(getPageOfUsersRequest, Users.API.Models.Shared.ResourceUriType.NextPage) : null;        

        var pagination = new Users.API.Models.Shared.Pagination()
        {
            TotalCount = pageOfUserResponses.TotalCount,
            TotalPages = pageOfUserResponses.TotalPages,
            CurrentPage = pageOfUserResponses.CurrentPage,
            PageSize = pageOfUserResponses.PageSize,
            PreviousPageLink = previousPageLink,
            NextPageLink = nextPageLink
        };

        return pagination;
    }

    private async Task<Users.API.Models.Response.v1.UserResponse> GetUserResponse(Users.API.Models.Request.v1.GetUserRequest getUserRequest)
    {
        if (getUserRequest == null) throw new ArgumentNullException(nameof(getUserRequest));

        var getUserQuery = _mapper.Map<Users.Application.Queries.GetUserQuery>(getUserRequest);

        var user = await _mediator.Send(getUserQuery);
        if (user == null) return null;

        var userResponse = _mapper.Map<Users.API.Models.Response.v1.UserResponse>(user);

        return userResponse;
    }

    private IEnumerable<Users.API.Models.Response.v1.UserResponse> GetUserResponses(Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse> pageOfUserResponses)
    {
        return _mapper.Map<IEnumerable<Users.API.Models.Response.v1.UserResponse>>(pageOfUserResponses);
    }
}
