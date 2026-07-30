using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Services;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;
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

    private async Task<Parcel?> FindParcelAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Parcels
            .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber, cancellationToken);
    }

    private async Task<string> GenerateTrackingNumberAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        var prefix = $"SP{now:yyyyMM}";

        var parcelsThisMonth = await _dbContext.Parcels
            .CountAsync(p => p.CreatedDate.Year == now.Year && p.CreatedDate.Month == now.Month, cancellationToken);
    
        var counter = (parcelsThisMonth + 1).ToString("D2"); 
    
        return $"{prefix}{counter}";
    }

    private async Task<int?> FindCustomerIdByEmail(string email, CancellationToken cancellationToken = default)
    {
        var customerId = await _dbContext.Customers
            .Where(c => c.Email == email)
            .Select(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        return customerId == 0 ? null : customerId; 
    }
    
    public async Task<ParcelStatusResponse?> GetParcelStatusAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        var parcel = await FindParcelAsync(trackingNumber, cancellationToken);

        if (parcel == null)
        {
            return null;
        }
        
        return new ParcelStatusResponse(parcel.Status);
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

    public async Task<List<CustomerParcelDto>?> GetCustomerParcelsAsync(string customerEmail, CancellationToken cancellationToken = default)
    {
        var customerExists = await _dbContext.Customers
            .AnyAsync(c => c.Email == customerEmail, cancellationToken);

        if (!customerExists)
        {
            return null;
        }
        
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

    public async Task<CreateParcelResponse?> CreateParcelAsync(CreateParcelRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
    
        var trackingNumber = await GenerateTrackingNumberAsync(now, cancellationToken);
        var customerId = await FindCustomerIdByEmail(request.Sender.Email, cancellationToken);

        if (customerId == null)
        {
            return null;
        }
        
        var newParcel = new Parcel
        {
            TrackingNumber = trackingNumber,
            CustomerId = customerId.Value,
            RecipientName = request.Recipient.Name,
            Weight = request.Parcel.Weight,
            Width = request.Parcel.Width,
            Length = request.Parcel.Length,
            Height = request.Parcel.Height,
            ServiceType = request.Parcel.ServiceType,
            DeclaredValueInEuros = request.Parcel.DeclaredValue,
        
            Status = ParcelStatus.PendingPickup, 
            CreatedDate = now,
        
            RecipientAddress = new Address
            {
                City = request.Recipient.RecipientAddress.City,
                CountryCode = request.Recipient.RecipientAddress.CountryCode,
                PostalCode = request.Recipient.RecipientAddress.PostalCode,
                Street = request.Recipient.RecipientAddress.Street,
                StreetNumber = request.Recipient.RecipientAddress.StreetNumber
            }
        };

        _dbContext.Parcels.Add(newParcel);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateParcelResponse(newParcel.TrackingNumber, newParcel.Status);
    }

    public async Task<DeliveryChangeResponse?> ChangeDeliveryAsync(string trackingNumber, DeliveryChangeRequest request,
        CancellationToken cancellationToken = default)
    {
        var parcel = await FindParcelAsync(trackingNumber, cancellationToken);

        if (parcel == null)
        {
            return null;
        }
        
        // Create a delivery_change request
        return null;
    }

    public async Task<bool> ConfirmDeliveryAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        var parcel = await FindParcelAsync(trackingNumber, cancellationToken);

        if (parcel == null)
        {
            return false;
        }

        parcel.Status = ParcelStatus.Delivered;
        parcel.DeliveredDate = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}