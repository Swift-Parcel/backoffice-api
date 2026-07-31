using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class CaseSeeder : IEntitySeeder
{
    public int Order => 110;

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Cases.AnyAsync(cancellationToken))
            return;

        var customersByEmail = await SeedingLookupHelper.GetCustomerLookupByEmailAsync(dbContext, cancellationToken);
        var regionsByName = await SeedingLookupHelper.GetRegionLookupByNameAsync(dbContext, cancellationToken);
        var validHandlerIds = await SeedingLookupHelper.GetHandlerIdLookupAsync(dbContext, cancellationToken);
        var parcelsByTrackingNumber = await SeedingLookupHelper.GetTrackedParcelLookupByTrackingNumberAsync(dbContext, cancellationToken);
        var tagsByName = await SeedingLookupHelper.GetTrackedTagLookupByNameAsync(dbContext, cancellationToken);

        var legacyCases = await oldDbContext.Database
            .SqlQueryRaw<LegacyCaseDto>(@"
                SELECT 
                    id, case_number, title, description, case_type, status, priority, 
                    customer_email, customer_phone, customer_name, handler_id, parcel_tracking_numbers, created_date, 
                    updated_date, resolved_date, sla_deadline, region, channel, tags, 
                    is_escalated, escalated_to, resolution, satisfaction_score 
                FROM cases")
            .ToListAsync(cancellationToken);

        var newCases = new List<Case>();

        foreach (var oldCase in legacyCases)
        {
            
            var normalizedEmail = StringParserHelper.NormalizeEmailOrDefault(oldCase.customer_email);
            if (string.IsNullOrEmpty(normalizedEmail) || !customersByEmail.TryGetValue(normalizedEmail, out var customerId))
            {
                continue; 
            }

            int? handlerId = null;
            if (int.TryParse(oldCase.handler_id, out var parsedHandlerId) && 
                validHandlerIds.ContainsKey(parsedHandlerId))
            {
                handlerId = parsedHandlerId;
            }

            int regionId = 1;
            var normalizedRegionName = NormalizeRegionName(oldCase.region);

            if (!string.IsNullOrEmpty(normalizedRegionName) && 
                regionsByName.TryGetValue(normalizedRegionName, out var parsedRegionId))
            {
                regionId = parsedRegionId;
            }

            Enum.TryParse<CaseType>(oldCase.case_type, true, out var caseType);
            Enum.TryParse<CaseStatus>(oldCase.status?.Replace(" ", ""), true, out var status);
            Enum.TryParse<Priority>(oldCase.priority, true, out var priority);
            Enum.TryParse<Channel>(oldCase.channel, true, out var channel);

            var newCase = new Case
            {
                Id = StringParserHelper.ExtractIntegerId(oldCase.id),
                CaseNumber = oldCase.case_number,
                Title = oldCase.title,
                Description = oldCase.description,
                CaseType = caseType,
                Status = status,
                Priority = priority,
                Channel = channel,
                CustomerId = customerId,
                HandlerId = handlerId,
                RegionId = regionId,
                CreatedDate = TimestampParserHelper.ParseOrFallback(oldCase.created_date),
                UpdatedDate = TimestampParserHelper.ParseOrFallback(oldCase.updated_date),
                IsEscalated = StringParserHelper.ParseBoolean(oldCase.is_escalated),
                ResolvedDate = TimestampParserHelper.ParseOrFallback(oldCase.resolved_date),
                SlaDeadline = TimestampParserHelper.ParseOrFallback(oldCase.sla_deadline),
                Resolution = oldCase.resolution,
                SatisfactionScore = int.TryParse(oldCase.satisfaction_score, out var score) ? score : 0
            };

            // Many-to-Many: Parcels
            if (!string.IsNullOrWhiteSpace(oldCase.parcel_tracking_numbers))
            {
                var trackingNumbers = StringParserHelper.ParseCsvString(oldCase.parcel_tracking_numbers);
                foreach (var trackingNumber in trackingNumbers)
                {
                    if (parcelsByTrackingNumber.TryGetValue(trackingNumber, out var parcel))
                    {
                        newCase.Parcels.Add(parcel);
                    }
                }
            }

            // Many-to-Many: Tags
            if (!string.IsNullOrWhiteSpace(oldCase.tags))
            {
                var tagList = StringParserHelper.ParseCsvString(oldCase.tags);
                foreach (var tagName in tagList)
                {
                    if (tagsByName.TryGetValue(tagName, out var tag))
                    {
                        newCase.Tags.Add(tag);
                    }
                }
            }

            newCases.Add(newCase);
        }

        await dbContext.Cases.AddRangeAsync(newCases, cancellationToken);
    }
    
    private static string NormalizeRegionName(string? rawRegion)
    {
        if (string.IsNullOrWhiteSpace(rawRegion))
            return string.Empty;

        var trimmed = rawRegion.Trim();

        // Egyedi név-eltérés lekezelése (Vienna -> Wien)
        if (trimmed.Equals("Vienna", StringComparison.OrdinalIgnoreCase))
        {
            return "Wien";
        }

        return trimmed;
    }

    private record LegacyCaseDto(
        string id,
        string case_number,
        string title,
        string description,
        string case_type,
        string status,
        string priority,
        string customer_email,
        string customer_phone,
        string customer_name,
        string handler_id,
        string parcel_tracking_numbers,
        string created_date,
        string updated_date,
        string? resolved_date,
        string sla_deadline,
        string region,
        string channel,
        string tags,
        string is_escalated,
        string? escalated_to,
        string? resolution,
        string? satisfaction_score);
}