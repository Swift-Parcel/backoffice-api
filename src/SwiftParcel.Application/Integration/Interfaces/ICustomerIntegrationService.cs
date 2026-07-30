using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Integration.Interfaces;

public interface ICustomerIntegrationService
{
    Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}