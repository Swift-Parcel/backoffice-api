using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Tags;
using SwiftParcel.Application.Tags.Queries.GetTags;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ApiController
{
    /// <summary>
    /// Retrieves a paginated list of all tags. Supports optional filtering by name.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<TagDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTags(
        [FromQuery] string? nameFilter, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetTagsQuery(nameFilter, pageNumber, pageSize);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}