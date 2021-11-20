namespace Users.API.Read.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UsersController(
        ILogger<UsersController> logger,
        IMediator mediator,
        IMapper mapper)
    {
        _logger = logger;
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Get a single user by their Id
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>The requested user</returns>
    /// <response code="200">Success - OK - Returns the requested user</response>
    /// <response code="204">Success - No Content - No user matched the given identifier</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    [HttpGet("{userId:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Users.API.Models.Response.v1.UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Users.API.Models.Response.v1.UserResponse>> GetUser([FromRoute] Guid userId)
    {
        if (userId == default) return BadRequest();

        var getUserRequest = new Users.API.Models.Request.v1.GetUserRequest(userId);
            
        var getUserResponse = await GetUserResponse(getUserRequest);
            
        if (getUserResponse == null) return NoContent();
            
        return Ok(getUserResponse);
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

    /// <summary>
    /// Gets a page of users
    /// </summary>
    /// <param name="getUsersRequest">
    /// A GetUsersRequest object which contains fields for paging, searching, filtering, sorting and shaping user data</param>
    /// <returns>A page of users</returns>
    /// <response code="200">Success - OK - Returns the requested page of users</response>
    /// <response code="204">Success - No Content - No users matched given criteria</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Users.API.Models.Response.v1.UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<Users.API.Models.Response.v1.UserResponse>>> GetUsers([FromQuery] Users.API.Models.Request.v1.GetPageOfUsersRequest getUsersRequest)
    {
        if (getUsersRequest == null) return BadRequest();

        // var userResponses = await GetUserResponses(getUsersRequest);
        //
        // if (userResponses == null || !userResponses.Any()) return NoContent();
        //
        // return Ok(userResponses);

        var usersResponse = GetUsersResponses(getUsersRequest);
        if (!usersResponse.Any()) return NotFound();
        return Ok(usersResponse);
    }

    private IEnumerable<Users.API.Models.Response.v1.UserResponse> GetUsersResponses(Users.API.Models.Request.v1.GetPageOfUsersRequest getUsersRequest)
    {
        // TODO --> Replace with real implementation

        var userResponse = new Users.API.Models.Response.v1.UserResponse
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Morsley"
        };

        var usersResponse = new List<Users.API.Models.Response.v1.UserResponse> { userResponse };
        return usersResponse;
    }
}
