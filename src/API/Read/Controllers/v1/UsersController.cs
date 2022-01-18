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
    /// <param name="userId">The unique identifier for the user</param>
    /// <param name="fields">The fields required to shape the user</param>
    /// <returns>The requested user (shaped, if required)</returns>
    /// <response code="200">Success - OK - Returns the requested user</response>
    /// <response code="204">Success - No Content - No user matched the given identifier</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    /// <response code="422">Error - Unprocessable Entity - The GetUserRequest object contained invalid data</response>
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

        var getUserResponse = await GetUserResponse(getUserRequest);

        if (getUserResponse == null) return NoContent();

        var shapedUser = getUserResponse.ShapeData(getUserRequest.Fields);

        var shapedUserWithLinks = AddLinks(shapedUser, getUserRequest);

        return Ok(shapedUserWithLinks);
    }

    [HttpHead("{userId:guid}")]
    [ProducesResponseType(typeof(Users.API.Models.Response.v1.UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<Users.API.Models.Response.v1.UserResponse>> Head(Users.API.Models.Request.v1.GetUserRequest getUserRequest)
    {
        if (getUserRequest == null) return BadRequest();

        var getUserResponse = await GetUserResponse(getUserRequest);

        if (getUserResponse == null) return NoContent();

        var shapedUser = getUserResponse.ShapeData(getUserRequest.Fields);
        var linkedShapedUser = AddLinks(shapedUser, getUserRequest);

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

        var pagination = GetPagination(pageOfUserResponses, getPageOfUsersRequest);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

        return Ok(GetUserResponses(pageOfUserResponses).ShapeData(getPageOfUsersRequest.Fields));
    }

    /// <summary>
    /// Gets a page of users
    /// </summary>
    /// <param name="getUsersRequest">
    /// A GetUsersRequest object which contains fields for paging, searching, filtering, sorting and shaping user data</param>
    /// <returns>A page of users</returns>
    /// <response code="200">Success - OK - Returns the requested page of users</response>
    /// <response code="204">Success - No Content - No users matched given criteria</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    [HttpHead]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<Users.API.Models.Response.v1.UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<IEnumerable<Users.API.Models.Response.v1.UserResponse>>> Head(
        [FromQuery] Users.API.Models.Request.v1.GetPageOfUsersRequest getPageOfUsersRequest)
    {
        if (getPageOfUsersRequest == null) return BadRequest();

        var pageOfUserResponses = await GetPageOfUserResponses(getPageOfUsersRequest);

        if (!pageOfUserResponses.Any()) return NoContent();

        Response.ContentLength = CalculateContentLength(pageOfUserResponses);

        return Ok();
    }

    [HttpOptions]
    public IActionResult GetOptions()
    {
        Response.Headers.Add("Allow", "GET,HEAD,OPTIONS");
        return Ok();
    }

    private ExpandoObject AddLinks(IDictionary<string, object> shapedUser, Users.API.Models.Request.v1.GetUserRequest getUserRequest)
    {
        var links = CreateLinksForUser(getUserRequest);
        //shapedUser.TryAdd("Links", links);
        shapedUser.Add("Links", links);
        return shapedUser as ExpandoObject;
    }

    private long CalculateContentLength<T>(T obj) where T : class
    {
        var json = System.Text.Json.JsonSerializer.Serialize(obj);
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

    private IEnumerable<Link> CreateLinksForUser(Users.API.Models.Request.v1.GetUserRequest getUserRequest)
    {
        var links = new List<Link>();

        //string url;
        Link link;

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
            var getUserLink = GetUserLink(getUserRequest);
            //url = Url.Action(nameof(Users.API.Read.Controllers.v1.UsersController.Get), nameof(Users.API.Read.Controllers.v1.UsersController), new { userId });
            //url = Url.Link("GetUser", new { userId, getUserRequest });
            //link = new Link(url, "self", "GET");
            links.Add(getUserLink);
        //}

        //url = Url.Link("DeleteUser", new { userId });
        var deleteUserLink = DeleteUserLink(getUserRequest.Id);        
        links.Add(deleteUserLink);

        //url = Url.Link("CreateUser", new { userId });
        //link = new Link(url, "create_user", "POST");
        //links.Add(link);

        return links;
    }

    private string CreateUsersResourceUri(Users.API.Models.Request.v1.GetPageOfUsersRequest getPageOfUsersRequest, Users.API.Models.Shared.ResourceUriType type)
    {
        switch (type)
        {
            case Users.API.Models.Shared.ResourceUriType.PreviousPage:
                return Url.Link(
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
            case Users.API.Models.Shared.ResourceUriType.NextPage:
                return Url.Link(
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
            default:
                return Url.Link(
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
