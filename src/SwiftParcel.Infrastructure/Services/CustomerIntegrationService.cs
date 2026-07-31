using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Customers;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Infrastructure.Services;

public class CustomerIntegrationService : ICustomerIntegrationService
{
    private readonly AppDbContext _dbContext;

    public CustomerIntegrationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var existingCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);
        if (existingCustomer != null)
            return new CreateCustomerResponse(existingCustomer.RegisteredDate);

        var address = new Address
        {
            City = request.Address.City,
            PostalCode = request.Address.PostalCode,
            Street = request.Address.Street,
            StreetNumber = request.Address.StreetNumber,
            CountryCode = request.Address.CountryCode
        };
        
        var registeredDate = DateTime.UtcNow.Date;
        var customer = new Customer()
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Address = address,
            RegisteredDate = registeredDate,
            Vip = false,
            Notes = string.Empty
        };

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCustomerResponse(registeredDate);
    }
}