using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Helpers;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;
using AddressParserHelper = SwiftParcel.Infrastructure.Persistence.Seeding.Helpers.AddressParserHelper;

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
            var parsedSenderAddress = AddressParserHelper.SplitStringAddress(legacyParcel.sender_address);
            var senderAddress = new Address(
                parsedSenderAddress.Street ?? string.Empty,
                parsedSenderAddress.StreetNumber ?? string.Empty,
                parsedSenderAddress.City ?? string.Empty,
                parsedSenderAddress.PostalCode ?? string.Empty,
                parsedSenderAddress.CountryCode ?? string.Empty
            );
            
            var parsedRecipientAddress = AddressParserHelper.SplitStringAddress(legacyParcel.recipient_address);
            var recipientAddress = new Address(
                parsedRecipientAddress.Street ?? string.Empty,
                parsedRecipientAddress.StreetNumber ?? string.Empty,
                parsedRecipientAddress.City ?? string.Empty,
                parsedRecipientAddress.PostalCode ?? string.Empty,
                parsedRecipientAddress.CountryCode ?? string.Empty
            );
            
            var customerId = StringParserHelper.ExtractInteger(legacyParcel.customer_id);
            if (!existingCustomerIds.Contains(customerId))
                customerId = existingCustomerIds.FirstOrDefault();

            var dimensions = ((int Width, int Length, int Height)?)StringParserHelper.ParseDimensions(legacyParcel.dimensions);

            var hasDeliveredDate = TimestampParserHelper.TryParse(legacyParcel.delivered_date, out var deliveredDate);
            
            var newParcel = new Parcel
            {
                Id = StringParserHelper.ExtractInteger(legacyParcel.id),
                TrackingNumber = FormatHelper.FormatTrackingNumber(legacyParcel.tracking_number ?? string.Empty),
                CustomerId = customerId,
                SenderAddress = senderAddress,
                RecipientName = legacyParcel.recipient_name ?? string.Empty,
                RecipientAddress = recipientAddress,
                Weight = StringParserHelper.ParseWeight(legacyParcel.weight) ?? 0f,
                Width = dimensions?.Width ?? 0,
                Length = dimensions?.Length ?? 0,
                Height = dimensions?.Height ?? 0,
                Status = ParseParcelStatus(legacyParcel.status),
                CreatedDate = TimestampParserHelper.ParseOrFallback(legacyParcel.created_date),
                DeliveredDate = hasDeliveredDate ? deliveredDate : null,
                ServiceType = ParseServiceType(legacyParcel.service_type),
                DeclaredValueInEuros = (float)StringParserHelper.ExtractDecimal(legacyParcel.declared_value)
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
        string? id,
        string? tracking_number,
        string? sender_address,
        string? recipient_name,
        string? recipient_address,
        string? weight,
        string? dimensions,
        string? status,
        string? created_date,
        string? delivered_date,
        string? service_type,
        string? declared_value,
        string? customer_id);
}