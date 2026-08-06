using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.DTO.Users;
using SwiftParcel.Application.Users.Commands.ActivateUser;
using SwiftParcel.Application.Users.Commands.AdminResetPassword;
using SwiftParcel.Application.Users.Commands.ChangeMyPassword;
using SwiftParcel.Application.Users.Commands.CreateUser;
using SwiftParcel.Application.Users.Commands.DeactivateUser;
using SwiftParcel.Application.Users.Commands.UpdateUser;
using SwiftParcel.Application.Users.Queries.GetUserById;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ApiController
{
    /// <summary>
    /// Creates a new back-office user. Returns the ID of the newly created user.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves a specific user's details by their unique ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Partially updates a user's details. You only need to send the fields you wish to change.
    /// Enforces business rules regarding Region and Role assignments.
    /// </summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(
            id, 
            request.FullName, 
            request.RoleId, 
            request.RegionIds);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Deactivates a user (soft delete). Deactivated users cannot log in or be assigned new cases.
    /// </summary>
    [HttpPatch("{id:int}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeactivateUser([FromRoute] int id, CancellationToken cancellationToken)
    {
        var command = new DeactivateUserCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
    
    /// <summary>
    /// Activate a user.
    /// </summary>
    [HttpPatch("{id:int}/activate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActivateUser([FromRoute] int id, CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
    
    /// <summary>
    /// Allows any authenticated user to change their own password. Requires the current password.
    /// </summary>
    [HttpPut("me/password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeMyPassword([FromBody] ChangeMyPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
    
    /// <summary>
    /// Allows an Admin to forcefully reset another user's password without knowing their current password.
    /// </summary>
    [HttpPut("{id:int}/password/reset")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminResetPassword([FromRoute] int id, [FromBody] AdminResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new AdminResetPasswordCommand(id, request.NewPassword);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
