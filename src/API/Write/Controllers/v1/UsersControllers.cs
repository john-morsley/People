namespace Users.API.Write.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
     * POST --> CREATE                                                                                                     *
     ***********************************************************************************************************************/                                                                                                                     

    /// <summary>
    /// Create a user
    /// </summary>
    /// <param name="request">A CreateUserRequest object which contains all the necessary data to create a user</param>
    /// <returns>A URI to the newly created user in the header (location)</returns>
    /// <response code="201">Success - Created - The user was successfully created</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response> 
    [HttpPost]
    [MapToApiVersion("1.0")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Users.API.Models.Response.v1.UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateUser([FromBody] Users.API.Models.Request.v1.CreateUserRequest request)
    {
        if (request == null) return BadRequest();

        var response = AddUser(request);

        return CreatedAtAction(nameof(CreateUser), response);
    }

    /***********************************************************************************************************************
     * DELETE                                                                                                              *
     ***********************************************************************************************************************/

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>Nothing</returns>
    /// <response code="204">Success - No Content - User was successfully deleted</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    /// <response code="404">Error - Not Found - No user matched the given identifier</response>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteUser([FromRoute] Guid userId)
    {
        if (userId == default) return BadRequest();

        var request = new Users.API.Models.Request.v1.DeleteUserRequest(userId);
            
        DeleteUser(request);

        return NoContent();
    }

    /***********************************************************************************************************************
     * PUT -- UPSERT                                                                                                       *
     ***********************************************************************************************************************/

    /// <summary>
    /// Upsert a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="upsertUserRequest">
    /// An UpsertUserRequest object which contains all the
    /// data required to either update or create a user.
    /// </param>
    /// <returns>The upserted user</returns>
    /// <response code="200">Success - OK - The user was successfully updated</response>
    /// <response code="201">Success - Created - The user was successfully created</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    [HttpPut("{guid:userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

    public async Task<IActionResult> UpsertUser(
        [FromRoute] Guid userId,
        [FromBody] Users.API.Models.Request.v1.UpsertUserRequest upsertUserRequest)
    {
        if (upsertUserRequest == null) return BadRequest();

        if (await DoesUserExist(userId))
        {
            var updatedUser = await UpdateUser(userId, upsertUserRequest);
            return Ok(updatedUser);
        }

        var createUserRequest = new Users.API.Models.Request.v1.CreateUserRequest();
        return RedirectToAction(nameof(CreateUser), createUserRequest);
    }

    /***********************************************************************************************************************
     * PATCH --> UPSERT                                                                                                    *
     ***********************************************************************************************************************/

    /// <summary>
    /// Fully or partially update a user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="patchDocument">
    /// A JSON Patch Document detailing the full or partial updates to the user
    /// </param>
    /// <returns>The updated user</returns>
    /// <response code="200">Success - OK - The user was successfully updated</response>
    /// <response code="400">Error - Bad Request - It was not possible to bind the request JSON</response>
    /// <response code="404">Error - Not Found - No user matched the given identifier</response>
    /// <response code="422">Error - Unprocessable Entity - Unable to process the contained instructions.</response>
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
    public async Task<IActionResult> UpsertUser(
        [FromRoute] Guid userId,
        [FromBody] JsonPatchDocument<Users.API.Models.Request.v1.PartiallyUpsertUserRequest> patchDocument)
    {
        if (patchDocument == null) return BadRequest();

        var partiallyUpsertUserRequest = new Users.API.Models.Request.v1.PartiallyUpsertUserRequest(userId);
        patchDocument.ApplyTo(partiallyUpsertUserRequest, ModelState);
        if (!TryValidateModel(partiallyUpsertUserRequest))
        {
            return ValidationProblem(ModelState);
        }
            
        if (await DoesUserExist(userId))
        {
            var updatedUser = await PartiallyUpdateUser(userId, partiallyUpsertUserRequest);
            return Ok(updatedUser);
        }
            
        var createUserRequest = new Users.API.Models.Request.v1.CreateUserRequest();
        return RedirectToAction(nameof(CreateUser), createUserRequest);
            
        throw new NotImplementedException();
    }
        
    private Users.API.Models.Response.v1.UserResponse AddUser(Users.API.Models.Request.v1.CreateUserRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
            
        // Here's where we'll add the user...
            
        throw new NotImplementedException();
    }

    private void DeleteUser(Users.API.Models.Request.v1.DeleteUserRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        // Here's where we'll delete the user...

        throw new NotImplementedException();
    }

    private async Task<bool> DoesUserExist(Guid id)
    {
        await Task.CompletedTask;
            
        throw new NotImplementedException();
    }
        
    private async Task<Users.API.Models.Response.v1.UserResponse> UpdateUser(
        Guid userId,
        Users.API.Models.Request.v1.UpsertUserRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        // Update User...
        await Task.CompletedTask;
            
        throw new NotImplementedException();
    }

    private async Task<Users.API.Models.Response.v1.UserResponse> PartiallyUpdateUser(
        Guid userId,
        Users.API.Models.Request.v1.PartiallyUpsertUserRequest partiallyUpsertUserRequest)
    {
        if (partiallyUpsertUserRequest == null) throw new ArgumentNullException(nameof(partiallyUpsertUserRequest));
            
        // var partiallyUpdateUserCommand = _mapper.Map<PartiallyUpdateUserCommand>(partiallyUpsertUserRequest);
        //
        // var updatedUser = await _mediator.Send(partiallyUpdateUserCommand);
        //
        // var updatedUserResponse = _mapper.Map<UserResponse>(updatedUser);
        //
        // return updatedUserResponse;

        throw new NotImplementedException();
    }
}
