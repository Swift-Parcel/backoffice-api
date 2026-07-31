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

        var addressLookup = await SeedingLookupHelper.GetAddressLookupAsync(dbContext, cancellationToken);

        var existingCustomerIds = await dbContext.Customers
            .Select(c => c.Id)
            .ToHashSetAsync(cancellationToken);

        var legacyParcels = await oldDbContext.Database
            .SqlQueryRaw<LegacyParcelDto>("SELECT * FROM parcels")
            .ToListAsync(cancellationToken);

        var newParcels = new List<Parcel>();

        foreach (var legacyParcel in legacyParcels)
        {
            var parsedAddress = AddressParserHelper.SplitStringAddress(legacyParcel.recipient_address);

            var addressKey = SeedingLookupHelper.GenerateAddressKey(
                parsedAddress.City,
                parsedAddress.Street,
                parsedAddress.StreetNumber,
                parsedAddress.PostalCode,
                parsedAddress.CountryCode);

            var customerId = StringParserHelper.ExtractIntegerId(legacyParcel.customer_id);
            if (!existingCustomerIds.Contains(customerId))
                customerId = existingCustomerIds.FirstOrDefault();

            var dimensions = StringParserHelpers.ParseDimensionalValues(legacyParcel.dimensions);

            var newParcel = new Parcel
            {
                Id = StringParserHelper.ExtractIntegerId(legacyParcel.id),
                TrackingNumber = FormatHelper.FormatTrackingNumber(legacyParcel.tracking_number ?? string.Empty),
                CustomerId = customerId,
                RecipientName = legacyParcel.recipient_name ?? string.Empty,
                RecipientAddressId = addressLookup.GetValueOrDefault(addressKey),
                Weight = StringParserHelpers.ParseWeight(legacyParcel.weight) ?? 0f,
                Width = dimensions?.Width ?? 0,
                Length = dimensions?.Length ?? 0,
                Height = dimensions?.Height ?? 0,
                Status = ParseParcelStatus(legacyParcel.status),
                CreatedDate = TimestampParserHelper.ParseOrFallback(legacyParcel.created_date),
                DeliveredDate = TimestampParserHelper.ParseOrFallback(legacyParcel.delivered_date, DateTime.MinValue),
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

    private record LegacyParcelDto(
        string? id,
        string? tracking_number,
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