using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Infrastructure.Services.Mock;

public class MockParcelService : IParcelIntegrationService
{
    public Task<ParcelTrackingResponse?> GetParcelTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        if (trackingNumber == "999") 
        {
            return Task.FromResult<ParcelTrackingResponse?>(null);
        }

        var response = new ParcelTrackingResponse(
            ParcelStatus.InTransit,
            new LocationDto("Central Hub", "Budapest", "HU", "1000", 47.4979, 19.0402),
            new List<TrackingHistoryDto>
            {
                new TrackingHistoryDto(DateTime.UtcNow.AddDays(-1), ParcelStatus.InTransit, "Label created", new LocationDto(null, "Pétervására", "HU", "3250", 48.0167, 20.1000)),
                new TrackingHistoryDto(DateTime.UtcNow, ParcelStatus.InTransit, "Arrived at hub", new LocationDto("Central Hub", "Budapest", "HU", "1000", 47.4979, 19.0402))
            }
        );

        return Task.FromResult<ParcelTrackingResponse?>(response);
    }

    public Task<ParcelStatusResponse?> GetParcelStatusAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        if (trackingNumber == "999") 
        {
            return Task.FromResult<ParcelStatusResponse?>(null);
        }

        var response = new ParcelStatusResponse(
            ParcelStatus.DeliveryAttemptFailed
        );
        
        return Task.FromResult<ParcelStatusResponse?>(response);
    }

    public Task<DeliveryEstimateResponse?> GetDeliveryEstimateAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        if (trackingNumber == "999") 
        {
            return Task.FromResult<DeliveryEstimateResponse?>(null);
        }

        var response = new DeliveryEstimateResponse(
            EstimatedDelivery: DateTime.UtcNow.AddDays(2),
            TimeSlots: new List<Timeslot> { Timeslot.Morning, Timeslot.Afternoon }
        );

        return Task.FromResult<DeliveryEstimateResponse?>(response);
    }

    public Task<List<CustomerParcelDto>?> GetCustomerParcelsAsync(string customerEmail, CancellationToken cancellationToken = default)
    {
        if (customerEmail == "notfound@example.com")
        {
            return Task.FromResult<List<CustomerParcelDto>?>(null);
        }

        var response = new List<CustomerParcelDto>
        {
            new CustomerParcelDto(
                "SP-20260701",
                ParcelStatus.InTransit,
                new CustomerParcelSenderDto(
                    customerEmail,
                    new AddressDto("Budapest", "HU", "1111", "Műegyetem rkp.", "3")
                ),
                new CustomerParcelRecipientDto(
                    "John Doe",
                    new AddressDto("Eger", "HU", "3300", "Dobó István tér", "1")
                ),
                DateTime.UtcNow.AddDays(-2),
                ServiceType.Standard
            ),
            new CustomerParcelDto(
                "SP-20260702",
                ParcelStatus.Delivered,
                new CustomerParcelSenderDto(
                    customerEmail,
                    new AddressDto("Szeged", "HU", "6720", "Dóm tér", "1")
                ),
                new CustomerParcelRecipientDto(
                    "Jane Doe",
                    new AddressDto("Debrecen", "HU", "4024", "Kossuth tér", "1")
                ),
                DateTime.UtcNow.AddDays(-5),
                ServiceType.Express
            )
        };

        return Task.FromResult<List<CustomerParcelDto>?>(response);
    }

    public Task<CreateParcelResponse?> CreateParcelAsync(CreateParcelRequest request, CancellationToken cancellationToken = default)
    {
        if (request?.Sender?.Email == "error@example.com")
        {
            return Task.FromResult<CreateParcelResponse?>(null);
        }

        var trackingNumber = $"SP-{DateTime.UtcNow:yyyyMM}99";
        var response = new CreateParcelResponse(trackingNumber, ParcelStatus.PendingPickup);

        return Task.FromResult<CreateParcelResponse?>(response);
    }

    public Task<DeliveryChangeResponse?> ChangeDeliveryAsync(string trackingNumber, DeliveryChangeRequest request, CancellationToken cancellationToken = default)
    {
        if (trackingNumber == "999")
        {
            return Task.FromResult<DeliveryChangeResponse?>(null);
        }

        var response = new DeliveryChangeResponse("CASE-98765");

        return Task.FromResult<DeliveryChangeResponse?>(response);
    }

    public Task<bool> ConfirmDeliveryAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        if (trackingNumber == "999")
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}