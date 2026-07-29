using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Parsers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class CustomerSeeder : IEntitySeeder
{
    public int Order => 6;
    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Customers.AnyAsync(cancellationToken))
        {
            return;
        }
        
        var addressLookup = await SeedingLookupHelper.GetAddressLookupAsync(dbContext, cancellationToken);
        
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
            
            var newCustomer = new Customer
            {
                Id = StringParserHelper.ExtractIntegerId(legacyCustomer.id),
                Name = legacyCustomer.name,
                Email = legacyCustomer.email,
                Phone = legacyCustomer.phone,
                AddressId = addressLookup.GetValueOrDefault(addressKey),
                RegisteredDate = TimestampParserHelper.ParseOrFallback(legacyCustomer.registered_date),
                Vip = StringParserHelper.ParseBoolean(legacyCustomer.vip),
                Notes = legacyCustomer.notes
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
}