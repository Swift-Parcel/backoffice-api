using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Reports.Queries.GetAverageResolutionTime;
using SwiftParcel.Application.Reports.Queries.GetHandlerWorkload;
using SwiftParcel.Application.Reports.Queries.GetOpenCasesByType;
using SwiftParcel.Application.Reports.Queries.GetSlaBreaches;

namespace SwiftParcel.Api.Controllers;

[Route("api/reports")]
public class ReportsController : ApiController
{
    [HttpGet("cases-by-type")]
    public async Task<IActionResult> GetOpenCasesByType(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetOpenCasesByTypeQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("sla-breaches")]
    public async Task<IActionResult> GetSlaBreaches(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSlaBreachesQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("average-resolution-time")]
    public async Task<IActionResult> GetAverageResolutionTime(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAverageResolutionTimeQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("handler-workload")]
    public async Task<IActionResult> GetHandlerWorkload(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetHandlerWorkloadQuery(), cancellationToken);
        return HandleResult(result);
    }
}