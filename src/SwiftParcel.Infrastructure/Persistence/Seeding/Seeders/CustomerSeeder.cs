using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Parsers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class CustomerSeeder : IEntitySeeder
{
    public int Order => 65;

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Customers.AnyAsync(cancellationToken))
        {
            return;
        }

        var addressLookup = await SeedingLookupHelper.GetAddressLookupAsync(dbContext, cancellationToken);
        

        int fallbackAddressId = addressLookup.Values.First();

        var legacyCustomers = await oldDbContext.Database
            .SqlQueryRaw<LegacyCustomerDto>("SELECT * FROM customers")
            .ToListAsync(cancellationToken);

        var newCustomers = new List<Customer>();

        var processedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var legacyCustomer in legacyCustomers)
        {
            var normalizedEmail = StringParserHelper.NormalizeEmailOrDefault(legacyCustomer.email);
            
            if (string.IsNullOrEmpty(normalizedEmail) || !processedEmails.Add(normalizedEmail))
            {
                continue;
            }
            
            var parsedAddress = AddressParserHelper.SplitStringAddress(legacyCustomer.address);

            var addressKey = SeedingLookupHelper.GenerateAddressKey(
                parsedAddress.City, 
                parsedAddress.Street, 
                parsedAddress.StreetNumber, 
                parsedAddress.PostalCode, 
                parsedAddress.CountryCode);

            int addressId = addressLookup.TryGetValue(addressKey, out var foundAddressId) 
                ? foundAddressId 
                : fallbackAddressId;

            var newCustomer = new Customer
            {
                Id = StringParserHelper.ExtractInteger(legacyCustomer.id),
                Name = legacyCustomer.name,
                Email = normalizedEmail,
                Phone = legacyCustomer.phone,
                AddressId = addressId,
                RegisteredDate = TimestampParserHelper.ParseOrFallback(legacyCustomer.registered_date),
                Vip = StringParserHelper.ParseBoolean(legacyCustomer.vip),
                Notes = legacyCustomer.notes
            };

            newCustomers.Add(newCustomer);
        }

        int nextId = newCustomers.Count > 0 ? newCustomers.Max(c => c.Id) + 1 : 1;
        
        var legacyOrphanCustomers = await oldDbContext.Database
            .SqlQueryRaw<LegacyCaseDto>(@"
                SELECT 
                    customer_email,
                    MAX(customer_name) AS customer_name,
                    MAX(customer_phone) AS customer_phone
                FROM cases
                WHERE customer_email IS NOT NULL 
                  AND TRIM(customer_email) <> ''
                  AND customer_email NOT IN (SELECT email FROM customers WHERE email IS NOT NULL)
                GROUP BY customer_email")
            .ToListAsync(cancellationToken);

        foreach (var legacyOrphanCustomer in legacyOrphanCustomers)
        {
            var normalizedEmail = StringParserHelper.NormalizeEmailOrDefault(legacyOrphanCustomer.customer_email);
            
            if (string.IsNullOrEmpty(normalizedEmail) || !processedEmails.Add(normalizedEmail))
            {
                continue;
            }
            
            var newCustomer = new Customer
            {
                Id = nextId++,
                Name = legacyOrphanCustomer.customer_name,
                Email = normalizedEmail,
                Phone = ContactInfoParserHelper.NormalizePhoneNumberOrDefault(legacyOrphanCustomer.customer_phone),
                AddressId = fallbackAddressId
            };

            newCustomers.Add(newCustomer);
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
        string? notes);

    private record LegacyCaseDto(
        string customer_name,
        string customer_email,
        string customer_phone);
}