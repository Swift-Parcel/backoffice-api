using SwiftParcel.Application.DTO.Customers;
using SwiftParcel.Application.Integration.Interfaces;

namespace SwiftParcel.Infrastructure.Services.Mock;

public class MockCustomerService : ICustomerIntegrationService
{
    public Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new CreateCustomerResponse(DateTime.UtcNow));
    }
}