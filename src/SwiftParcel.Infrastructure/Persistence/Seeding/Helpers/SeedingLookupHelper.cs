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
}