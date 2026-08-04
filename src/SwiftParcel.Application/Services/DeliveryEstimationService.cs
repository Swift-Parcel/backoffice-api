using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Services;

public class DeliveryEstimationService : IDeliveryEstimationService
{
    private readonly IAppDbContext _context;

    public DeliveryEstimationService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<DeliveryEstimateResponse> CalculateForParcelAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        var parcel = await _context.Parcels
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber, cancellationToken) 
            ?? throw new InvalidOperationException($"Parcel '{trackingNumber}' not found.");

        int businessDaysToAdd = parcel.ServiceType switch
        {
            ServiceType.SameDay => 0,
            ServiceType.Express => 1,
            _ => 3
        };

        var estimatedDeliveryDate = AddBusinessDays(parcel.CreatedDate, businessDaysToAdd);

        var availableTimeSlots = GetAvailableTimeSlots(parcel.Id);

        return new DeliveryEstimateResponse(estimatedDeliveryDate, availableTimeSlots);
    }

    private static DateTime AddBusinessDays(DateTime current, int days)
    {
        while (days > 0)
        {
            current = current.AddDays(1);
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
                days--;
        }
        return current;
    }

    private static List<Timeslot> GetAvailableTimeSlots(int parcelId)
    {
        return (parcelId % 2 == 0)
            ? new List<Timeslot> { Timeslot.Morning, Timeslot.Afternoon }
            : new List<Timeslot> { Timeslot.Afternoon, Timeslot.Evening };
    }
}