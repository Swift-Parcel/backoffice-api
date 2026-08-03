using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;

public static class SeedingLookupHelper
{
    /// <summary>
    /// Maps Region Name to Region Id
    /// </summary>
    public static async Task<Dictionary<string, int>> GetRegionLookupByNameAsync(
        AppDbContext dbContext, 
        CancellationToken ct = default)
    {
        return await dbContext.Regions
            .AsNoTracking()
            .ToDictionaryAsync(
                r => r.Name, 
                r => r.Id, 
                StringComparer.OrdinalIgnoreCase, 
                ct);
    }

    /// <summary>
    /// Maps Username to User Id
    /// </summary>
    public static async Task<Dictionary<string, int>> GetUserLookupByUsernameAsync(
        AppDbContext dbContext, 
        CancellationToken ct = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Username != null)
            .ToDictionaryAsync(
                u => u.Username!, 
                u => u.Id, 
                StringComparer.OrdinalIgnoreCase, 
                ct);
    }
    
    /// <summary>
    /// Maps Handler Name (User Name) to Handler Id
    /// </summary>
    public static async Task<Dictionary<string, int>> GetHandlerLookupByNameAsync(
        AppDbContext dbContext, 
        CancellationToken ct = default)
    {
        return await dbContext.Handlers
            .AsNoTracking()
            .Select(h => new { Name = h.User.FullName, HandlerId = h.Id })
            .ToDictionaryAsync(
                x => x.Name, 
                x => x.HandlerId, 
                StringComparer.OrdinalIgnoreCase, 
                ct);
    }

    /// <summary>
    /// Maps User Email to User Id
    /// </summary>
    public static async Task<Dictionary<string, int>> GetUserLookupByEmailAsync(
        AppDbContext dbContext, 
        CancellationToken ct = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Email != null)
            .ToDictionaryAsync(
                u => u.Email!, 
                u => u.Id, 
                StringComparer.OrdinalIgnoreCase, 
                ct);
    }

    /// <summary>
    /// Maps Customer Email to Customer Id
    /// </summary>
    public static async Task<Dictionary<string, int>> GetCustomerLookupByEmailAsync(
        AppDbContext dbContext, 
        CancellationToken ct = default)
    {
        return await dbContext.Customers
            .AsNoTracking()
            .Where(c => c.Email != null)
            .ToDictionaryAsync(
                c => c.Email!, 
                c => c.Id, 
                StringComparer.OrdinalIgnoreCase, 
                ct);
    }
    
    /// <summary>
    /// Maps Customer Email to Customer Id
    /// </summary>
    public static async Task<Dictionary<string, int>> GetCustomerLookupByPhoneAsync(
        AppDbContext dbContext, 
        CancellationToken ct = default)
    {
        return await dbContext.Customers
            .AsNoTracking()
            .Where(c => c.Phone != null)
            .ToDictionaryAsync(
                c => c.Phone!, 
                c => c.Id, 
                StringComparer.OrdinalIgnoreCase, 
                ct);
    }
    
    /// <summary>
    /// Maps Customer Email to Customer Id
    /// </summary>
    public static async Task<Dictionary<string, int>> GetCustomerLookupByNameAsync(
        AppDbContext dbContext, 
        CancellationToken ct = default)
    {
        return await dbContext.Customers
            .AsNoTracking()
            .Where(c => c.FullName != null)
            .ToDictionaryAsync(
                c => c.FullName!, 
                c => c.Id, 
                StringComparer.OrdinalIgnoreCase, 
                ct);
    }

    /// <summary>
    /// Generates a deterministic key from address components to ensure matching lookups.
    /// </summary>
    public static string GenerateAddressKey(string? city, string? street, string? streetNumber, string? postalCode, string? countryCode)
    {
        return $"{city?.Trim()}|{street?.Trim()}|{streetNumber?.Trim()}|{postalCode?.Trim()}|{countryCode?.Trim()}";
    }
    
    /// <summary>
    /// Maps Handler Id to itself for existence validation. Highly optimized.
    /// </summary>
    public static async Task<Dictionary<int, int>> GetHandlerIdLookupAsync(
        AppDbContext dbContext, 
        CancellationToken ct = default)
    {
        return await dbContext.Handlers
            .AsNoTracking()
            .Select(h => h.Id)
            .ToDictionaryAsync(
                id => id, 
                id => id, 
                ct);
    }

    /// <summary>
    /// Maps Tracking Number to Parcel entity.
    /// </summary>
    public static async Task<Dictionary<string, Parcel>> GetTrackedParcelLookupByTrackingNumberAsync(
        AppDbContext dbContext, 
        CancellationToken ct = default)
    {
        return await dbContext.Parcels
            .Where(p => p.TrackingNumber != null)
            .ToDictionaryAsync(
                p => p.TrackingNumber!, 
                StringComparer.OrdinalIgnoreCase, 
                ct);
    }

    /// <summary>
    /// Maps Tag Name to Tag entity
    /// </summary>
    public static async Task<Dictionary<string, Tag>> GetTrackedTagLookupByNameAsync(
        AppDbContext dbContext, 
        CancellationToken ct = default)
    {
        return await dbContext.Tags
            .Where(t => t.Name != null)
            .ToDictionaryAsync(
                t => t.Name!, 
                StringComparer.OrdinalIgnoreCase, 
                ct);
    }
}