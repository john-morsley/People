using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Users.API.Read.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }
        
        /*
         * GET --> Get a single User
         */

        /// <summary>
        /// Get a user
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
        public async Task<IActionResult> GetUser([FromRoute] Guid userId)
        {
            if (userId == default) return BadRequest();

            //var getUserRequest = new GetUserRequest(id);
            //
            // var getUserResponse = await GetUserResponse(getUserRequest);
            //
            // if (getUserResponse == null) return NoContent();
            //
            // return Ok(getUserResponse);

            var userResponse = GetUserResponse(userId);
            if (userResponse == null) return NotFound();
            return Ok(userResponse);
            
        }

        private Users.API.Models.Response.v1.UserResponse GetUserResponse(Guid userId)
        {
            // TODO --> Replace with real implementation

            var userResponse = new Users.API.Models.Response.v1.UserResponse
            {
                Id = userId,
                FirstName = "John",
                LastName = "Morsley"
            };

            return userResponse;
        }

        /*
         * GET --> Get a page of Users
         */

        /// <summary>
        /// Get a page of users
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
        public async Task<IActionResult> GetUsers([FromQuery] Users.API.Models.Request.v1.GetPageOfUsersRequest getUsersRequest)
        {
            if (getUsersRequest == null) return BadRequest();

            // var userResponses = await GetUserResponses(getUsersRequest);
            //
            // if (userResponses == null || !userResponses.Any()) return NoContent();
            //
            // return Ok(userResponses);

            var usersResponse = GetUsersResponse(getUsersRequest);
            if (!usersResponse.Any()) return NotFound();
            return Ok(usersResponse);
        }

        private IEnumerable<Users.API.Models.Response.v1.UserResponse> GetUsersResponse(Users.API.Models.Request.v1.GetPageOfUsersRequest getUsersRequest)
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
}