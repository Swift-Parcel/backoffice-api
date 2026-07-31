using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Models;

namespace SwiftParcel.Application.Integration.Interfaces;

public interface IParcelIntegrationService
{
    // GET /api/integration/parcels/{trackingNumber}/status  
    Task<ParcelStatusResponse?> GetParcelStatusAsync(string trackingNumber, CancellationToken cancellationToken = default);
    
    // GET /api/integration/parcels/{trackingNumber}
    Task<ParcelTrackingResponse?> GetParcelTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default);
    
    // GET /api/integration/parcels/{trackingNumber}/delivery-estimate
    Task<DeliveryEstimateResponse?> GetDeliveryEstimateAsync(string trackingNumber, CancellationToken cancellationToken = default);
    
    // GET /api/integration/parcels?customerEmail=...
    Task<List<CustomerParcelDto>?> GetCustomerParcelsAsync(string customerEmail, CancellationToken cancellationToken = default);
    
    // POST /api/integration/parcels
    Task<CreateParcelResponse?> CreateParcelAsync(CreateParcelRequest request, CancellationToken cancellationToken = default);
   
    // POST /api/integration/parcels/{trackingNumber}/delivery-change
    Task<DeliveryChangeResponse?> ChangeDeliveryAsync(string trackingNumber, DeliveryChangeRequest request, CancellationToken cancellationToken = default);
    
    // PATCH /api/integration/parcels/{trackingNumber}/confirm-delivery
    Task<bool> ConfirmDeliveryAsync(string trackingNumber, CancellationToken cancellationToken = default);}