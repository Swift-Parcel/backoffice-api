using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Models;

namespace SwiftParcel.Application.Integration.Interfaces;

public interface IParcelIntegrationService
{
    // GET /api/integration/parcels/{trackingNumber}/status  -- created for testing
    Task<ParcelStatusResponse?> GetParcelStatusAsync(string trackingNumber, CancellationToken cancellationToken = default);
    
    // GET /api/integration/parcels/{trackingNumber}
    Task<ParcelTrackingResponse?> GetParcelTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default);
    
    // GET /api/integration/parcels/{trackingNumber}/delivery-estimate
    Task<DeliveryEstimateResponse?> GetDeliveryEstimateAsync(string trackingNumber, CancellationToken cancellationToken = default);
    
    // GET /api/integration/parcel?{customerEmail}=...
    Task<CustomerParcelDto?> GetCustomerParcelAsync(string trackingNumber, CancellationToken cancellationToken = default);
    
    // POST /api/integration/parcels
    Task<CreateParcelRequest?> GetCreateParcelRequestAsync(string trackingNumber, CancellationToken cancellationToken = default);
    Task<CreateParcelResponse?> CreateParcelAsync(CreateParcelRequest createParcelRequest, CancellationToken cancellationToken = default);
   
    // POST /api/integration/parcels/{trackingNumber}/delivery-change
    Task<DeliveryChangeRequest?> GetDeliveryChangeRequestAsync(string trackingNumber, CancellationToken cancellationToken = default);
    Task<DeliveryEstimateResponse?> CreateDeliveryEstimateAsync(string trackingNumber, CancellationToken cancellationToken = default);
    
    // PATCH /api/integration/parcels/{trackingNumber}/confirm-delivery
    Task<ConfirmDeliveryRequest?> GetConfirmDeliveryRequestAsync(string trackingNumber, CancellationToken cancellationToken = default);
}