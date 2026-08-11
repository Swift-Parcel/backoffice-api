using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Regions;
using SwiftParcel.Application.Regions.Queries.GetRegions;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/regions")]
public class RegionsController : ApiController
{
    /// <summary>
    /// Retrieves a paginated list of all regions. Supports optional filtering by name.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<RegionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegions(
        [FromQuery] string? nameFilter, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetRegionsQuery(nameFilter, pageNumber, pageSize);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}