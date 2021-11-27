using System.Text.Json;
using Users.API.Models.Shared;

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

    /***********************************************************************************************************************
     * GET --> SINGLE                                                                                                      *
     ***********************************************************************************************************************/

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
    public async Task<ActionResult<Users.API.Models.Response.v1.UserResponse>> GetUserById([FromRoute] Guid userId)
    {
        if (userId == default) return BadRequest();

        var getUserRequest = new Users.API.Models.Request.v1.GetUserRequest(userId);
            
        var getUserResponse = await GetUserResponse(getUserRequest);
            
        if (getUserResponse == null) return NoContent();

        return Ok(getUserResponse);
    }

    /***********************************************************************************************************************
     * HEAD --> SINGLE                                                                                                     *
     ***********************************************************************************************************************/

    [HttpHead("{userId:guid}")]
    [ProducesResponseType(typeof(Users.API.Models.Response.v1.UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Users.API.Models.Response.v1.UserResponse>> HeadUserById([FromRoute] Guid userId)
    {
        if (userId == default) return BadRequest();

        var getUserRequest = new Users.API.Models.Request.v1.GetUserRequest(userId);

        var getUserResponse = await GetUserResponse(getUserRequest);

        if (getUserResponse == null) return NoContent();

        Response.ContentLength = CalculateContentLength(getUserResponse);

        return Ok();
    }

    /***********************************************************************************************************************
     * GET --> MULTIPLE                                                                                                    *
     ***********************************************************************************************************************/

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
    [ProducesResponseType(typeof(Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>> GetPageOfUsers(
        [FromQuery] Users.API.Models.Request.v1.GetPageOfUsersRequest getPageOfUsersRequest)
    {
        if (getPageOfUsersRequest == null) return BadRequest();

        var pageOfUserResponses = await GetPageOfUserResponses(getPageOfUsersRequest);

        if (!pageOfUserResponses.Any()) return NoContent();

        return Ok(pageOfUserResponses);
    }

    /***********************************************************************************************************************
     * HEAD --> MULTIPLE                                                                                                   *
     ***********************************************************************************************************************/

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
    [ProducesResponseType(typeof(Users.API.Models.Shared.PagedList < Users.API.Models.Response.v1.UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>> HeadPageOfUsers(
        [FromQuery] Users.API.Models.Request.v1.GetPageOfUsersRequest getPageOfUsersRequest)
    {
        if (getPageOfUsersRequest == null) return BadRequest();

        var pageOfUserResponses = await GetPageOfUserResponses(getPageOfUsersRequest);

        if (!pageOfUserResponses.Any()) return NoContent();

        Response.ContentLength = CalculateContentLength(pageOfUserResponses);

        return Ok();
    }

    private long CalculateContentLength(Models.Response.v1.UserResponse getUserResponse)
    {
        var json = JsonSerializer.Serialize(getUserResponse);
        return json.Length;
    }

    private long CalculateContentLength(PagedList<Models.Response.v1.UserResponse> pageOfUserResponses)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new Users.API.Models.Shared.PagedListJsonConverter() }
        };
        var json = JsonSerializer.Serialize(pageOfUserResponses, options);
        return json.Length;
    }

    private async Task<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>> GetPageOfUserResponses(Users.API.Models.Request.v1.GetPageOfUsersRequest getPageOfUsersRequest)
    {
        if (getPageOfUsersRequest == null) throw new ArgumentNullException(nameof(getPageOfUsersRequest));

         var getPageOfUsersQuery = _mapper.Map<GetPageOfUsersQuery>(getPageOfUsersRequest);

        var pageOfUsers = await _mediator.Send(getPageOfUsersQuery);

        var pageOfUserResponses = _mapper.Map<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>(pageOfUsers);

        return pageOfUserResponses;
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

}
