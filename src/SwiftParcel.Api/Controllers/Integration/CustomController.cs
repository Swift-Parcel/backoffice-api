using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.Integration.Interfaces;

namespace SwiftParcel.Api.Controllers.Integration;

[ApiController]
[Route("api/integration/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerIntegrationService _customerIntegrationService;

    public CustomersController(ICustomerIntegrationService customerIntegrationService)
    {
        _customerIntegrationService = customerIntegrationService;
    }

    /// <summary>
    /// Create a customer record upon registration.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateCustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await _customerIntegrationService.CreateCustomerAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}