using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Services;

public class DeliveryEstimationService : IDeliveryEstimationService
{
    public Task<DeliveryEstimateResponse> CalculateForParcelAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        // Dummy Data Implementation
        var estimatedDeliveryDate = DateTime.UtcNow.AddDays(2);
        var availableTimeSlots = new List<Timeslot> 
        { 
            Timeslot.Morning, 
            Timeslot.Afternoon 
        };

        var response = new DeliveryEstimateResponse(estimatedDeliveryDate, availableTimeSlots);
        
        return Task.FromResult(response);
    }
}