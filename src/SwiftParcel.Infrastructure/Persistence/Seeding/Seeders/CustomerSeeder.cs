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

        if (addressLookup.Count == 0)
        {
            throw new InvalidOperationException("Nem található egyetlen cím sem az addressLookup-ban. Az AddressSeeder-nek előbb kell lefutnia!");
        }

        // Első elérhető cím ID-ja tartalékként (biztosan létezik az adatbázisban)
        int fallbackAddressId = addressLookup.Values.First();

        var legacyCustomers = await oldDbContext.Database
            .SqlQueryRaw<LegacyCustomerDto>("SELECT * FROM customers")
            .ToListAsync(cancellationToken);

        var newCustomers = new List<Customer>();

        foreach (var legacyCustomer in legacyCustomers)
        {
            var parsedAddress = AddressParserHelper.SplitStringAddress(legacyCustomer.address);

            var addressKey = SeedingLookupHelper.GenerateAddressKey(
                parsedAddress.City, 
                parsedAddress.Street, 
                parsedAddress.StreetNumber, 
                parsedAddress.PostalCode, 
                parsedAddress.CountryCode);

            // Ha nem találja a szótárban, a fallback cím ID-ját kapja (0 helyett)
            int addressId = addressLookup.TryGetValue(addressKey, out var foundAddressId) 
                ? foundAddressId 
                : fallbackAddressId;

            var newCustomer = new Customer
            {
                Id = StringParserHelper.ExtractIntegerId(legacyCustomer.id),
                Name = legacyCustomer.name,
                Email = legacyCustomer.email,
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
            .SqlQueryRaw<LegacyCaseDto>(@"SELECT customer_name, customer_email, customer_phone FROM cases
                                          WHERE customer_name not in (select name from customers)
                                          AND customer_email not in (select email from customers)
                                          AND customer_phone not in (select phone from customers)")
            .ToListAsync(cancellationToken);

        foreach (var legacyOrphanCustomer in legacyOrphanCustomers)
        {
            var newCustomer = new Customer
            {
                Id = nextId++,
                Name = legacyOrphanCustomer.customer_name,
                Email = StringParserHelper.NormalizeEmailOrDefault(legacyOrphanCustomer.customer_email),
                Phone = StringParserHelper.NormalizePhoneNumberOrDefault(legacyOrphanCustomer.customer_phone),
                AddressId = fallbackAddressId // Az árva vásárlóknak is kötelező érvényes AddressId-t adni
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