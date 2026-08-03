using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Customers.Commands.CreateCustomer;

namespace SwiftParcel.Api.Controllers;


public class CustomersController : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}