using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Customers;

namespace SwiftParcel.Application.Integration.Interfaces;

public interface ICustomerIntegrationService
{
    Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}