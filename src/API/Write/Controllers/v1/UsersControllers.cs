namespace Users.API.Write.Controllers.v1;

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
        IMapper mapper)
    {
        _logger = logger;
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Create a user
    /// </summary>
    /// <param name="addUserRequest">An AddUserRequest object which contains all the necessary data to create a user</param>
    /// <returns>A URI to the newly added user in the header (location)</returns>
    /// <response code="201">Success - Added - The user was successfully Added</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response> 
    [HttpPost(Name = "CreateUser")]
    [MapToApiVersion("1.0")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Users.API.Models.Response.v1.UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] Users.API.Models.Request.v1.AddUserRequest addUserRequest)
    {
        if (addUserRequest == null) return BadRequest();

        var userResponse = await AddUser(addUserRequest);

        return Created($"http://localhost/api/v1/users/{userResponse.Id}", userResponse);
    }

    /// <summary>
    /// Delete an existing user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>Nothing</returns>
    /// <response code="204">Success - No Content - User was successfully deleted</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    /// <response code="404">Error - Not Found - No user matched the given identifier</response>
    [HttpDelete("{userId:guid}", Name = "DeleteUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid userId)
    {
        if (userId == default) return BadRequest();

        if (!await DoesUserExist(userId)) return NotFound();

        await DeleteUser(userId);

        return NoContent();
    }

    /// <summary>
    /// Update an existing user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="updateUserRequest">An UpdateUserRequest object which contains the data required to update a user.</param>
    /// <returns>The updated user</returns>
    /// <response code="200">Success - OK - The user was successfully updated</response>
    /// <response code="404">Error - Not Found - Could not find the corresponding user for the given identifier</response>
    /// <response code="422">Error - Unprocessable Entity - It was not possible to bind the request JSON</response>
    [HttpPut("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid userId,
        [FromBody] Users.API.Models.Request.v1.UpdateUserRequest updateUserRequest)
    {
        if (updateUserRequest == null) return BadRequest();

        // As we are upserting, we either update or add...
        if (await DoesUserExist(userId))
        {
            // Update existing user...
            var updatedUserResponse = await UpdateUser(userId, updateUserRequest);
            return Ok(updatedUserResponse);
        }

        // Add new user...
        var addedUserResponse = await AddUser(userId, updateUserRequest);
        return Created($"http://localhost/api/v1/users/{addedUserResponse.Id}", addedUserResponse);
    }

    /// <summary>
    /// Fully or partially update an existing user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="patchDocument">A JSON Patch Document detailing the full or partial updates to the user</param>
    /// <returns>The updated user</returns>
    /// <response code="200">Success - OK - The user was successfully updated</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    /// <response code="404">Error - Not Found - Could not find the corresponding user for the given identifier</response>
    /// <response code="422">Error - Unprocessable Entity - Unable to process the contained instructions</response>
    /// <remarks>
    /// Sample request (this request updates the user's first name): \
    /// PATCH /users/id \
    /// [ \
    ///     { \
    ///         "op": "replace", \
    ///         "path": "/firstname", \
    ///         "value": "Bob" \
    ///     } \
    /// ] \
    /// </remarks>
    [HttpPatch("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Patch(
        [FromRoute] Guid userId,
        [FromBody] JsonPatchDocument<Users.API.Models.Request.v1.PartiallyUpdateUserRequest> patchDocument)
    {
        if (patchDocument == null) return BadRequest();

        Users.API.Models.Request.v1.PartiallyUpdateUserRequest partiallyUpdateUserRequest;

        // As we are upserting, we either update or add...
        if (await DoesUserExist(userId))
        {
            // Update existing user...
            var user = await GetUser(userId);
            partiallyUpdateUserRequest = _mapper.Map<Users.API.Models.Request.v1.PartiallyUpdateUserRequest>(user);
            patchDocument.ApplyTo(partiallyUpdateUserRequest, ModelState);
            if (!TryValidateModel(partiallyUpdateUserRequest))
            {
                return ValidationProblem(ModelState);
            }
            var updatedUserResponse = await UpdateUser(partiallyUpdateUserRequest);
            return Ok(updatedUserResponse);
        }
        
        // Add new user...
        partiallyUpdateUserRequest = new Users.API.Models.Request.v1.PartiallyUpdateUserRequest(userId);
        patchDocument.ApplyTo(partiallyUpdateUserRequest, ModelState);
        if (!TryValidateModel(partiallyUpdateUserRequest))
        {
            return ValidationProblem(ModelState);
        }

        var addUserRequest = _mapper.Map<Users.API.Models.Request.v1.AddUserRequest>(partiallyUpdateUserRequest);
        var userResponse = await AddUser(addUserRequest);
        return Created($"http://localhost/api/v1/users/{userResponse.Id}", userResponse);
    }

    /// <summary>
    /// List the HTTP methods allowed
    /// </summary>
    [HttpOptions]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetOptions()
    {
        Response.Headers.Add("Allow", "DELETE,OPTIONS,PATCH,POST,PUT");
        return Ok();
    }

    public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
    {
        var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
        return (ActionResult) options.Value.InvalidModelStateResponseFactory(ControllerContext);
    }

    private async Task<Users.API.Models.Response.v1.UserResponse> AddUser(Users.API.Models.Request.v1.AddUserRequest addUserRequest)
    {
        var addUserCommand = _mapper.Map<Users.Application.Commands.AddUserCommand>(addUserRequest);
        var user = await _mediator.Send(addUserCommand);
        var userResponse = _mapper.Map<Users.API.Models.Response.v1.UserResponse>(user);
        return userResponse;
    }

    private async Task<Users.API.Models.Response.v1.UserResponse> AddUser(Guid userId, Users.API.Models.Request.v1.UpdateUserRequest updateUserRequest)
    {
        var addUserCommand = _mapper.Map<Users.Application.Commands.AddUserCommand>(updateUserRequest);
        addUserCommand.Id = userId;
        var user = await _mediator.Send(addUserCommand);
        var userResponse = _mapper.Map<Users.API.Models.Response.v1.UserResponse>(user);
        return userResponse;
    }

    private async Task DeleteUser(Guid userId)
    {
        var deleteUserRequest = new Users.API.Models.Request.v1.DeleteUserRequest(userId);

        var deleteUserCommand = _mapper.Map<Users.Application.Commands.DeleteUserCommand>(deleteUserRequest);

        await _mediator.Send(deleteUserCommand);
    }

    private async Task<bool> DoesUserExist(Guid userId)
    {
        var userExistsQuery = new Application.Queries.UserExistsQuery(userId);

        var doesUserExist = await _mediator.Send(userExistsQuery);
        
        return doesUserExist;
    }

    private async Task<Users.Domain.Models.User> GetUser(Guid userId)
    {
        var getUserRequest = new Users.API.Models.Request.v1.GetUserRequest() { Id = userId };
        var getUserQuery = _mapper.Map<Users.Application.Queries.GetUserQuery>(getUserRequest);
        var user = await _mediator.Send(getUserQuery);
        return user;
    }

    private async Task<Users.API.Models.Response.v1.UserResponse> UpdateUser(Guid userId, Users.API.Models.Request.v1.UpdateUserRequest updateUserRequest)
    {
        var updateUserCommand = _mapper.Map<Application.Commands.UpdateUserCommand>(updateUserRequest);
        updateUserCommand.Id = userId;
        var updatedUser = await _mediator.Send(updateUserCommand);
        var updatedUserResponse = _mapper.Map<Users.API.Models.Response.v1.UserResponse>(updatedUser);
        return updatedUserResponse;
    }

    private async Task<Users.API.Models.Response.v1.UserResponse> UpdateUser(Users.API.Models.Request.v1.PartiallyUpdateUserRequest partiallyUpdateUserRequest)
    {
        var partiallyUpdateUserCommand = _mapper.Map<Users.Application.Commands.PartiallyUpdateUserCommand>(partiallyUpdateUserRequest);
        var updatedUser = await _mediator.Send(partiallyUpdateUserCommand);
        var updatedUserResponse = _mapper.Map<Users.API.Models.Response.v1.UserResponse>(updatedUser);
        return updatedUserResponse;
    }
}
