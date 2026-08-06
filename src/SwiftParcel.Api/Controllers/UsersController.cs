using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.DTO.Users;
using SwiftParcel.Application.Users.Commands.CreateUser;
using SwiftParcel.Application.Users.Commands.DeactivateUser;
using SwiftParcel.Application.Users.Commands.UpdateUser;
using SwiftParcel.Application.Users.Queries.GetUserById;

namespace SwiftParcel.Api.Controllers;

[Route("api/users")]
public class UsersController : ApiController
{
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
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

    [HttpPatch("{id:int}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser([FromRoute] int id, CancellationToken cancellationToken)
    {
        var command = new DeactivateUserCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
