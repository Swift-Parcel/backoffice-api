using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Authentication.Commands.Login;
using SwiftParcel.Application.Common.Models.Authentication;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ApiController
{
    /// <summary>
    /// Authenticates a user and returns a JWT token if successful.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}