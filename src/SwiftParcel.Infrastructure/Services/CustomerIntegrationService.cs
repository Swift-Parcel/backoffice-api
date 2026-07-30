using SwiftParcel.Application.DTO;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Infrastructure.Persistence;

namespace SwiftParcel.Infrastructure.Services;

public class CustomerIntegrationService : ICustomerIntegrationService
{
    private readonly AppDbContext _dbContext;

    public CustomerIntegrationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}