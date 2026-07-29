using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Parsers;

public class CustomerSeeder : IEntitySeeder
{
    public int Order => 70;

    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Customers.AnyAsync(cancellationToken))
            return;

        var legacyCustomers = await dbContext.Database
            .SqlQueryRaw<LegacyCustomerDto>(@"
                SELECT 
                    id, name, email, phone, address, 
                    registered_date, vip, notes 
                FROM customers")
            .ToListAsync(cancellationToken);

        var newCustomers = new List<Customer>();
        var processedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var oldCustomer in legacyCustomers)
        {
            var email = oldCustomer.email?.Trim() ?? string.Empty;

            // Email deduplication
            if (!string.IsNullOrEmpty(email) && !processedEmails.Add(email))
                continue;

            // Parse VIP Boolean
            bool isVip = oldCustomer.vip?.Trim().ToLowerInvariant() is "yes" or "y" or "true" or "1";

            var customer = new Customer
            {
                Id = StringParserHelper.ExtractIntegerId(oldCustomer.id),
                Name = oldCustomer.name ?? string.Empty,
                Email = email,
                Phone = oldCustomer.phone ?? string.Empty,
                Address = AddressParserHelper.SplitStringAddress(oldCustomer.address),
                RegisteredDate = TimestampParserHelper.ParseOrFallback(oldCustomer.registered_date),
                Vip = isVip,
                Notes = oldCustomer.notes ?? string.Empty
            };

            newCustomers.Add(customer);
        }

        await dbContext.Customers.AddRangeAsync(newCustomers, cancellationToken);
    }

    private record LegacyCustomerDto(
        string id,
        string name,
        string email,
        string phone,
        string address,
        string registered_date,
        string vip,
        string notes);
}