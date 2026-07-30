using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Integration.Models;
using SwiftParcel.Application.Services;
using SwiftParcel.Infrastructure.Persistence;

namespace SwiftParcel.Infrastructure.Services;

public class ParcelIntegrationService : IParcelIntegrationService
{
    private readonly AppDbContext _dbContext;
    private readonly IDeliveryEstimationService _estimationService;

    public ParcelIntegrationService(AppDbContext dbContext, IDeliveryEstimationService estimationService)
    {
        _dbContext = dbContext;
        _estimationService = estimationService;
    }

    public async Task<ParcelStatusResponse?> GetParcelStatusAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        var response = await _dbContext.Parcels
            .Where(p => p.TrackingNumber == trackingNumber)
            .Select(p => new ParcelStatusResponse(p.Status))
            .FirstOrDefaultAsync(cancellationToken);

        return response;
    }

    public async Task<ParcelTrackingResponse?> GetParcelTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DeliveryEstimateResponse?> GetDeliveryEstimateAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        var parcelExists = await _dbContext.Parcels
            .AnyAsync(p => p.TrackingNumber == trackingNumber, cancellationToken);

        if (!parcelExists)
        {
            return null; 
        }

        var estimate = await _estimationService.CalculateForParcelAsync(trackingNumber, cancellationToken);

        return estimate;
    }

    public async Task<List<CustomerParcelDto>> GetCustomerParcelsAsync(string customerEmail, CancellationToken cancellationToken = default)
    {
        var parcels = await _dbContext.Parcels
            .Where(p => p.Customer.Email == customerEmail)
            .Select(p => new CustomerParcelDto(
                p.TrackingNumber,
                p.Status,
                new CustomerParcelSenderDto(
                    p.Customer.Email,
                    new AddressDto(
                        p.Customer.Address.City,
                        p.Customer.Address.CountryCode,
                        p.Customer.Address.PostalCode,
                        p.Customer.Address.Street,
                        p.Customer.Address.StreetNumber
                    )
                ),
                new CustomerParcelRecipientDto(
                    string.Empty, // TODO: Ask Java if this is really needed
                    p.RecipientName,
                    new AddressDto(
                        p.RecipientAddress.City,
                        p.RecipientAddress.CountryCode,
                        p.RecipientAddress.PostalCode,
                        p.RecipientAddress.Street,
                        p.RecipientAddress.StreetNumber
                    )
                ),
                p.CreatedDate,
                p.ServiceType
            ))
            .ToListAsync(cancellationToken);

        return parcels;
    }

    public async Task<CreateParcelRequest?> GetCreateParcelRequestAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CreateParcelResponse?> CreateParcelAsync(CreateParcelRequest createParcelRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DeliveryChangeRequest?> GetDeliveryChangeRequestAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DeliveryEstimateResponse?> CreateDeliveryEstimateAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ConfirmDeliveryRequest?> GetConfirmDeliveryRequestAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}