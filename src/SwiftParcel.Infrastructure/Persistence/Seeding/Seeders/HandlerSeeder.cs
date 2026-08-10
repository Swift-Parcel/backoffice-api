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
            if (!StringParserHelper.ParseBoolean(legacyHandler.is_active))
            {
                continue;
            }
            
            var newHandler = new Handler(
                id : StringParserHelper.ExtractInteger(legacyHandler.id),
                userId : userLookup.GetValueOrDefault(legacyHandler.email),
                department : legacyHandler.department,
                hireDate : TimestampParserHelper.ParseOrFallback(legacyHandler.hire_date),
                maxCases : StringParserHelper.ExtractInteger(legacyHandler.max_cases),
                isActive : StringParserHelper.ParseBoolean(legacyHandler.is_active)
            );
            
            newHandlers.Add(newHandler);
        }
        
        //admin
        var adminUser = await dbContext.Users.FirstAsync(u => u.Username == "admin", cancellationToken);
        var id = legacyHandlers.Select(h => StringParserHelper.ExtractInteger(h.id)).DefaultIfEmpty(0).Max() + 1;
        var adminHandler = new Handler(
            id: id,
            userId: adminUser.Id,
            department: "Escalations",
            hireDate: DateTime.UtcNow.AddYears(-5),
            maxCases: 10,
            isActive:true
        );
        
        newHandlers.Add(adminHandler);
        
        await dbContext.Handlers.AddRangeAsync(newHandlers, cancellationToken);
    }
    
    private sealed record LegacyHandlerDto(
        string id,
        string email,
        string department,
        string is_active,
        string hire_date,
        string max_cases);
}