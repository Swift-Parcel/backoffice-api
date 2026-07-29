using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class HandlerSeeder : IEntitySeeder
{
    public int Order => 80;
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Handlers.AnyAsync(cancellationToken))
        {
            return;
        }
        
        var legacyHandlers = await dbContext.Database
            .SqlQueryRaw<LegacyHandlerDto>("SELECT id, email, department, is_active, hire_date, max_cases FROM handlers")
            .ToListAsync(cancellationToken);

        var userLookup = await SeedingLookupHelper.GetUserLookupByEmailAsync(dbContext, cancellationToken);
        
        var newHandlers = new List<Handler>();
        
        foreach (var legacyHandler in legacyHandlers)
        {
            if (!StringParserHelper.ParseBoolean(legacyHandler.is_active))
            {
                continue;
            }
            
            var newHandler = new Handler()
            {
                Id = StringParserHelper.ExtractIntegerId(legacyHandler.id),
                UserId = userLookup[legacyHandler.email],
                Department = legacyHandler.department,
                HireDate = TimestampParserHelper.ParseOrFallback(legacyHandler.hire_date),
                MaxCases = StringParserHelper.ExtractInteger(legacyHandler.max_cases)
            };
            
            newHandlers.Add(newHandler);
        }
        
        await dbContext.Handlers.AddRangeAsync(newHandlers, cancellationToken);
    }
    
    private record LegacyHandlerDto(
        string id,
        string email,
        string department,
        string is_active,
        string hire_date,
        string max_cases);
}