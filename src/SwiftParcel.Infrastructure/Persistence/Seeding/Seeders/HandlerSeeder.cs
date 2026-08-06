using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class HandlerSeeder : IEntitySeeder
{
    public int Order => 80;
    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Handlers.AnyAsync(cancellationToken))
        {
            return;
        }
        
        var legacyHandlers = await oldDbContext.Database
            .SqlQueryRaw<LegacyHandlerDto>("SELECT id, email, department, is_active, hire_date, max_cases FROM handlers")
            .ToListAsync(cancellationToken);

        var userLookup = await SeedingLookupHelper.GetUserLookupByEmailAsync(dbContext, cancellationToken);
        
        var newHandlers = new List<Handler>();
        
        foreach (var legacyHandler in legacyHandlers)
        {
            var newHandler = new Handler()
            {
                Id = StringParserHelper.ExtractInteger(legacyHandler.id),
                UserId = userLookup.GetValueOrDefault(legacyHandler.email),
                Department = legacyHandler.department,
                HireDate = TimestampParserHelper.ParseOrFallback(legacyHandler.hire_date),
                MaxCases = StringParserHelper.ExtractInteger(legacyHandler.max_cases),
                IsActive = StringParserHelper.ParseBoolean(legacyHandler.is_active)
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