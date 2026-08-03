using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Customers.Commands.CreatePortalCustomer;
using SwiftParcel.Application.DTO.Customers;

namespace SwiftParcel.Api.Controllers.Integration;

[ApiController]
[Route("api/integration/[controller]")]
public class CustomersController : ApiController
{
    /// <summary>
    /// Create a customer record upon registration.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateCustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreatePortalCustomerCommand request, 
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}