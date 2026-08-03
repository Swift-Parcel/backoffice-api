using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Helpers;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Services;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Integration.Models;
using SwiftParcel.Infrastructure.Persistence;

namespace SwiftParcel.Infrastructure.Services;

public class ParcelService : IParcelService
{
    private readonly AppDbContext _dbContext;
    private readonly IDeliveryEstimationService _estimationService;
    private readonly IWebhookClient  _webhookClient;
    private readonly ICaseService _caseService;

    public ParcelService(AppDbContext dbContext, IDeliveryEstimationService estimationService,  IWebhookClient webhookClient, ICaseService _caseService)
    {
        _dbContext = dbContext;
        _estimationService = estimationService;
        _webhookClient = webhookClient;
        _caseService = _caseService;
    }

    private async Task<Parcel?> FindParcelAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Parcels
            .Include(p => p.Customer)
            .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber, cancellationToken);
    }

    private async Task<string> GenerateTrackingNumberAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        var prefix = $"SP-{now:yyyyMM}";

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
    
    private static ParcelStatus MapEuroTrackStatus(string euroTrackStatusCode, ParcelStatus fallbackStatus)
    {
        return euroTrackStatusCode switch
        {
            "PICKED_UP" => ParcelStatus.PickedUp,
            "ARRIVED_AT_FACILITY" => ParcelStatus.InTransit,
            "DEPARTED_FACILITY" => ParcelStatus.InTransit,
            "IN_TRANSIT" => ParcelStatus.InTransit,
            "ARRIVED_AT_DELIVERY_DEPOT" => ParcelStatus.InTransit,
            "OUT_FOR_DELIVERY" => ParcelStatus.OutForDelivery,
            "DELIVERED" => ParcelStatus.Delivered,
            "DELIVERY_ATTEMPT_FAILED" => ParcelStatus.DeliveryAttemptFailed,
            "EXCEPTION" => ParcelStatus.Damaged,
            "LOST_IN_NETWORK" => ParcelStatus.Lost,
            _ => fallbackStatus
        };
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
        var formattedTrackingNumber = FormatHelper.FormatTrackingNumber(trackingNumber);
        
        var parcel = await _dbContext.Parcels
            .Select(p => new { p.TrackingNumber, p.Status })
            .FirstOrDefaultAsync(p => p.TrackingNumber == formattedTrackingNumber, cancellationToken);
        
        if (parcel == null)
        {
            return null; 
        }

        var filePath = Path.Combine(AppContext.BaseDirectory, "Integration", "Mocks", "eurotrack_mock.json");
        var jsonString = await File.ReadAllTextAsync(filePath, cancellationToken);
        var mockApiData = JsonSerializer.Deserialize<EuroTrackResponseDto>(jsonString);

        var shipmentData = mockApiData?.Shipments.FirstOrDefault(s => 
            FormatHelper.FormatTrackingNumber(s.TrackingNumber) == formattedTrackingNumber);

        var trackingHistory = new List<TrackingHistoryDto>();
        LocationDto? currentLocation = null;
        
        var currentParcelStatus = shipmentData != null 
            ? MapEuroTrackStatus(shipmentData.CurrentStatus, parcel.Status) 
            : parcel.Status;
        
        if (shipmentData != null && shipmentData.Events.Any())
        {
            var sortedEvents = shipmentData.Events.OrderBy(e => e.Timestamp).ToList();

            foreach (var e in sortedEvents)
            {
                var eventLocation = new LocationDto(
                    Facility: e.Location.Facility,
                    City: e.Location.City ?? "Unknown",
                    CountryCode: e.Location.CountryCode ?? "Unknown",
                    PostalCode: e.Location.PostalCode ?? "Unknown",
                    Lat: e.Location.Lat ?? 0.0,
                    Lon: e.Location.Lon ?? 0.0
                );

                trackingHistory.Add(new TrackingHistoryDto(
                    Timestamp: e.Timestamp,
                    ParcelStatus: MapEuroTrackStatus(e.StatusCode, parcel.Status), 
                    Description: e.Description,
                    Location: eventLocation
                ));
            }

            currentLocation = trackingHistory.Last().Location;
        }

        currentLocation ??= new LocationDto(null, "Unknown", "Unknown", "Unknown", 0.0, 0.0);

        return new ParcelTrackingResponse(
            ParcelStatus: currentParcelStatus, 
            Location: currentLocation,
            TrackingHistory: trackingHistory
        );
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
            (
                request.Recipient.RecipientAddress.City,
                request.Recipient.RecipientAddress.CountryCode,
                request.Recipient.RecipientAddress.PostalCode,
                request.Recipient.RecipientAddress.Street,
                request.Recipient.RecipientAddress.StreetNumber
            )
        };

        _dbContext.Parcels.Add(newParcel);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        // Webhook
        await _webhookClient.NotifyParcelStatusChangedAsync(newParcel.TrackingNumber, newParcel.Status, cancellationToken);

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
        
        var countryCode = await _dbContext.Customers
            .Where(c => c.Id == parcel.CustomerId)
            .Select(c => c.Address.CountryCode)
            .FirstOrDefaultAsync(cancellationToken);

        var regionId = await _dbContext.Regions
            .Where(r => r.CountryCode == countryCode && r.IsActive)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        var caserequest = new CreateCaseRequest
        (
            CustomerEmail : parcel.Customer.Email,
            TrackingNumbers : [trackingNumber],
            CaseType : CaseType.DeliveryChange,
            CaseTitle : "Delivery Change",
            RegionId : regionId,
            Channel :  Channel.Portal,
            Description : $"{request.Date} - {request.Timeslot}"
        );
        
        var caseResponse = await _caseService.CreateCaseAsync(caserequest, cancellationToken);
        
        if (caseResponse is null)
            return null;
        
        
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
        
        // Webhook
        await _webhookClient.NotifyParcelStatusChangedAsync(parcel.TrackingNumber, parcel.Status, cancellationToken);
        return true;
    }
}