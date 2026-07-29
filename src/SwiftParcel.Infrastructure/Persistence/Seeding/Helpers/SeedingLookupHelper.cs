using Microsoft.EntityFrameworkCore;

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
            .Where(c => c.Name != null)
            .ToDictionaryAsync(
                c => c.Name!, 
                c => c.Id, 
                StringComparer.OrdinalIgnoreCase, 
                ct);
    }
    
    /// <summary>
    /// Maps a deterministic composite address key to Address Id
    /// </summary>
    public static async Task<Dictionary<string, int>> GetAddressLookupAsync(
        AppDbContext dbContext, 
        CancellationToken ct = default)
    {
        var addresses = await dbContext.Addresses
            .AsNoTracking()
            .Select(a => new { a.Id, a.City, a.Street, a.StreetNumber, a.PostalCode, a.CountryCode })
            .ToListAsync(ct);

        return addresses.ToDictionary(
            a => GenerateAddressKey(a.City, a.Street, a.StreetNumber, a.PostalCode, a.CountryCode), 
            a => a.Id, 
            StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Generates a deterministic key from address components to ensure matching lookups.
    /// </summary>
    public static string GenerateAddressKey(string? city, string? street, string? streetNumber, string? postalCode, string? countryCode)
    {
        return $"{city?.Trim()}|{street?.Trim()}|{streetNumber?.Trim()}|{postalCode?.Trim()}|{countryCode?.Trim()}";
    }
}