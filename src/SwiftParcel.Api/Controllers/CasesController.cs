using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Cases.Commands.CreateCase;

namespace SwiftParcel.Api.Controllers;

public class CasesController : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCaseCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}