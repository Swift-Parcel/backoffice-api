using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Helpers;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Parsers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class ParcelSeeder : IEntitySeeder
{
    public int Order => 100;

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Parcels.AnyAsync(cancellationToken))
            return;
        
        var existingCustomerIds = await dbContext.Customers
            .Select(c => c.Id)
            .ToHashSetAsync(cancellationToken);

        var legacyParcels = await oldDbContext.Database
            .SqlQueryRaw<LegacyParcelDto>("SELECT * FROM parcels")
            .ToListAsync(cancellationToken);

        var newParcels = new List<Parcel>();

        foreach (var legacyParcel in legacyParcels)
        {
            var parsedAddress = AddressParserHelper.SplitStringAddress(legacyParcel.RecipientAddress);
            
            var recipientAddress = new Address(
                parsedAddress.Street ?? string.Empty,
                parsedAddress.StreetNumber ?? string.Empty,
                parsedAddress.City ?? string.Empty,
                parsedAddress.PostalCode ?? string.Empty,
                parsedAddress.CountryCode ?? string.Empty
            );
            
            var customerId = StringParserHelper.ExtractInteger(legacyParcel.CustomerId);
            if (!existingCustomerIds.Contains(customerId))
                customerId = existingCustomerIds.FirstOrDefault();

            var dimensions = ((int Width, int Length, int Height)?)StringParserHelper.ParseDimensions(legacyParcel.Dimensions);

            var newParcel = new Parcel
            {
                Id = StringParserHelper.ExtractInteger(legacyParcel.Id),
                TrackingNumber = FormatHelper.FormatTrackingNumber(legacyParcel.TrackingNumber ?? string.Empty),
                CustomerId = customerId,
                RecipientName = legacyParcel.RecipientName ?? string.Empty,
                RecipientAddress = recipientAddress,
                Weight = StringParserHelper.ParseWeight(legacyParcel.Weight) ?? 0f,
                Width = dimensions?.Width ?? 0,
                Length = dimensions?.Length ?? 0,
                Height = dimensions?.Height ?? 0,
                Status = ParseParcelStatus(legacyParcel.Status),
                CreatedDate = TimestampParserHelper.ParseOrFallback(legacyParcel.CreatedDate),
                DeliveredDate = TimestampParserHelper.ParseOrFallback(legacyParcel.DeliveredDate, DateTime.MinValue),
                ServiceType = ParseServiceType(legacyParcel.ServiceType),
                DeclaredValueInEuros = (float)StringParserHelper.ExtractDecimal(legacyParcel.DeclaredValue)
            };

            newParcels.Add(newParcel);
        }

        await dbContext.Parcels.AddRangeAsync(newParcels, cancellationToken);
    }

    private static ParcelStatus ParseParcelStatus(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return ParcelStatus.PendingPickup;

        var normalized = input.Trim().ToUpperInvariant().Replace(" ", "_").Replace("-", "_");

        return normalized switch
        {
            "PENDING_PICKUP" or "PENDINGPICKUP" => ParcelStatus.PendingPickup,
            "PICKED_UP" or "PICKEDUP" => ParcelStatus.PickedUp,
            "IN_TRANSIT" or "INTRANSIT" or "TRANSIT" => ParcelStatus.InTransit,
            "OUT_FOR_DELIVERY" or "OUTFORDELIVERY" => ParcelStatus.OutForDelivery,
            "DELIVERED" => ParcelStatus.Delivered,
            "DELIVERY_ATTEMPT_FAILED" or "DELIVERYATTEMPTFAILED" => ParcelStatus.DeliveryAttemptFailed,
            "LOST" => ParcelStatus.Lost,
            "DAMAGED" => ParcelStatus.Damaged,
            _ => ParcelStatus.PendingPickup
        };
    }

    private static ServiceType ParseServiceType(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return ServiceType.Standard;

        var normalized = input.Trim().ToUpperInvariant().Replace(" ", "_").Replace("-", "_");

        return normalized switch
        {
            "STANDARD" => ServiceType.Standard,
            "EXPRESS" => ServiceType.Express,
            "SAME_DAY" or "SAMEDAY" => ServiceType.SameDay,
            _ => ServiceType.Standard
        };
    }

    private sealed record LegacyParcelDto(
        string? Id,
        string? TrackingNumber,
        string? RecipientName,
        string? RecipientAddress,
        string? Weight,
        string? Dimensions,
        string? Status,
        string? CreatedDate,
        string? DeliveredDate,
        string? ServiceType,
        string? DeclaredValue,
        string? CustomerId);
}