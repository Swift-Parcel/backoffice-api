using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Customers.Queries.GetCustomerByEmail;

namespace SwiftParcel.Api.Controllers;

public class CustomersInternal : ApiController
{
    [HttpGet("api/customers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerByEmail(
        [FromQuery] GetCustomerByEmailRequest request, CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerByEmailQuery(request.Email);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}