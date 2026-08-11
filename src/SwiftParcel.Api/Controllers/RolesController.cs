using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Roles;
using SwiftParcel.Application.Roles.Queries.GetRoles;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/roles")]
public class RolesController : ApiController
{
    /// <summary>
    /// Retrieves a paginated list of all roles. Supports optional filtering by name.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles(
        [FromQuery] string? nameFilter, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetRolesQuery(nameFilter, pageNumber, pageSize);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}