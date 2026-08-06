using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Authentication;
using SwiftParcel.Infrastructure.Services;

namespace SwiftParcel.Infrastructure.Persistence.Seeding;

public class TestDataSeeder
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher _passwordHasher = new PasswordHasher();
    private readonly CaseNumberGenerator _caseNumberGenerator;

    public TestDataSeeder(AppDbContext context)
    {
        _context = context;
        _caseNumberGenerator = new CaseNumberGenerator(context);
    }

    public async Task SeedAsync()
    {
        if (await _context.Users.AnyAsync(u => u.Username == "supervisor"))
        {
            return;
        }

        await SeedRegionAsync();

        await SeedUsersForRolesAsync();
        await SeedHandlerFullAsync();
        await SeedParcelsAsync();
        await SeedCasesAsync();
    }

    private async Task SeedRegionAsync()
    {
        if (!await _context.Regions.AnyAsync())
        {
            var region = new Region
            {
                Name = "Debrecen",
                CountryCode = "HU",
                BusinessHoursStart = new TimeOnly(8, 0),
                BusinessHoursEnd = new TimeOnly(17, 0),
                IsActive = true,
                ManagerEmail = "manager@swiftparcel.com"
            };
            _context.Regions.Add(region);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedUsersForRolesAsync()
    {
        var newUsers = new List<User>();

        var readOnlyUser = new User()
        {
            Username = "readonly",
            PasswordHash = _passwordHasher.HashPassword("ReadOnly123!"),
            FullName = "Readonly User",
            RoleId = await _context.Roles
                .Where(r => r.Name == "Read-Only")
                .Select(r => r.Id)
                .FirstOrDefaultAsync(),
            Email = "readonly@example.com",
            CreatedById = await _context.Users
                .Where(u => u.Username == "system")
                .Select(u => u.Id)
                .FirstOrDefaultAsync(),
        };
        newUsers.Add(readOnlyUser);

        var @operator = new User()
        {
            Username = "operator",
            PasswordHash = _passwordHasher.HashPassword("Operator123!"),
            FullName = "Operator User",
            RoleId = await _context.Roles
                .Where(r => r.Name == "Operator")
                .Select(r => r.Id)
                .FirstOrDefaultAsync(),
            Email = "operator@example.com",
            CreatedById = await _context.Users
                .Where(u => u.Username == "system")
                .Select(u => u.Id)
                .FirstOrDefaultAsync(),
        };
        newUsers.Add(@operator);

        var supervisor = new User()
        {
            Username = "supervisor",
            PasswordHash = _passwordHasher.HashPassword("Supervisor123!"),
            FullName = "Supervisor User",
            RoleId = await _context.Roles
                .Where(r => r.Name == "Supervisor")
                .Select(r => r.Id)
                .FirstOrDefaultAsync(),
            Email = "supervisor@example.com",
            CreatedById = await _context.Users
                .Where(u => u.Username == "system")
                .Select(u => u.Id)
                .FirstOrDefaultAsync(),
        };
        newUsers.Add(supervisor);

        _context.Users.AddRange(newUsers);
        await _context.SaveChangesAsync();
    }

    private async Task SeedHandlerFullAsync()
    {
        var handlerFullUser = new User()
        {
            Username = "handlerfull",
            PasswordHash = _passwordHasher.HashPassword("HandlerFull123!"),
            FullName = "Handler Full User",
             RoleId = await _context.Roles
                 .Where(r => r.Name == "Operator")
                 .Select(r => r.Id)
                 .FirstOrDefaultAsync(),
            Email = "handlerfull@example.com",
            CreatedDate = DateTime.UtcNow
        };
        _context.Users.Add(handlerFullUser);
        await _context.SaveChangesAsync();

        var handlerFull = new Handler()
        {
            UserId = handlerFullUser.Id,
            Department = "Customer Support",
            HireDate = DateTime.UtcNow.AddYears(-2),
            MaxCases = 2
        };
        _context.Handlers.Add(handlerFull);

        var customer1 = new Customer()
        {
            FullName = "Customer 1",
            Email = "customer1@example.com",
            Phone = "123-456-7890",
            RegisteredDate = DateTime.UtcNow.AddYears(-1),
            Vip = false,
        };

        var customer2 = new Customer()
        {
            FullName = "Customer 2",
            Email = "customer2@example.com",
            Phone = "098-765-4321",
            RegisteredDate = DateTime.UtcNow.AddYears(-1),
            Vip = true,
        };

        _context.Customers.AddRange(customer1, customer2);
        await _context.SaveChangesAsync();

        var regionId = await _context.Regions.Select(r => r.Id).FirstAsync();

        var case1 = new Case()
        {
            CaseNumber = await _caseNumberGenerator.GenerateNextAsync(),
            Title = "Test Case 1 (Limit test)",
            Description = "This is a test case assigned to handlerfull.",
            CaseType = CaseType.Damaged,
            Status = CaseStatus.InProgress,
            Priority = Priority.Medium,
            Customer = customer1,
            Handler = handlerFull,
            CreatedDate = DateTime.UtcNow.AddDays(-1),
            UpdatedDate = DateTime.UtcNow,
            IsEscalated = false,
            SlaDeadline = DateTime.UtcNow.AddHours(48), 
            RegionId = regionId,
            Channel = Channel.Portal
        };

        var case2 = new Case()
        {
            CaseNumber = await _caseNumberGenerator.GenerateNextAsync(),
            Title = "Test Case 2 (Limit test)",
            Description = "Second case to max out handler capacity.",
            CaseType = CaseType.Delayed,
            Status = CaseStatus.InProgress,
            Priority = Priority.High,
            Customer = customer2,
            Handler = handlerFull,
            CreatedDate = DateTime.UtcNow.AddHours(-12),
            UpdatedDate = DateTime.UtcNow,
            SlaDeadline = DateTime.UtcNow.AddHours(48),
            RegionId = regionId,
            Channel = Channel.Phone
        };

        _context.Cases.AddRange(case1, case2);
        await _context.SaveChangesAsync();
    }

    private async Task SeedParcelsAsync()
    {
        var customer1 = await _context.Customers.FirstAsync(c => !c.Vip);
        var customer2 = await _context.Customers.FirstAsync(c => c.Vip);

        var parcels = new List<Parcel>
        {
            new Parcel
            {
                TrackingNumber = "SP-20261016", 
                Customer = customer1, 
                RecipientName = "John Doe",
                RecipientAddress = new Address
                (
                    Street : "Petofi Sandor",
                    StreetNumber :"123",
                    City : "Kiskunlachaza",
                    PostalCode : "1234",
                    CountryCode: "HU"
                ),
                Weight = 2.5f,
                Width = 30,
                Length = 40,
                Height = 20,
                Status = ParcelStatus.InTransit, 
                CreatedDate = DateTime.UtcNow.AddDays(-2),
                ServiceType = ServiceType.Standard,
                DeclaredValueInEuros = 100.0f,

            },
            new Parcel
            {
                TrackingNumber = "SP-20261017", 
                Customer = customer1,
                RecipientName = "Jane Smith",
                RecipientAddress = new Address
                (
                    Street : "Kossuth Lajos",
                    StreetNumber :"456",
                    City : "Debrecen",
                    PostalCode : "4000",
                    CountryCode: "HU"
                ),
                Weight = 1.0f,
                Width = 20,
                Length = 30,
                Height = 10,
                Status = ParcelStatus.Delivered, 
                CreatedDate = DateTime.UtcNow.AddDays(-5), 
                DeliveredDate = DateTime.UtcNow.AddDays(-1),
                ServiceType = ServiceType.Express,
                DeclaredValueInEuros = 50.0f,
            },
            new Parcel
            {
                TrackingNumber = "SP-20261018", 
                Customer = customer2, 
                RecipientName = "Alice Johnson",
                RecipientAddress = new Address
                (
                    Street : "Aradi Város",
                    StreetNumber :"789",
                    City : "Szeged",
                    PostalCode : "6724",
                    CountryCode: "HU"
                ),
                Weight = 15.2f,
                Width = 50,
                Length = 60,
                Height = 40,
                Status = ParcelStatus.DeliveryAttemptFailed, 
                CreatedDate = DateTime.UtcNow.AddDays(-7),
                ServiceType = ServiceType.Standard,
                DeclaredValueInEuros = 200.0f,
            },
            new Parcel
            {
                TrackingNumber = "SP-20261019", 
                Customer = customer2, 
                RecipientName = "Bob Brown",
                RecipientAddress = new Address
                (
                    Street : "Bahnhof",
                    StreetNumber :"456",
                    City : "Wien",
                    PostalCode : "2310",
                    CountryCode: "AT"
                ),
                Weight = 0.5f,
                Width = 50,
                Length = 60,
                Height = 40,
                Status = ParcelStatus.Lost, 
                CreatedDate = DateTime.UtcNow.AddDays(-10),
                ServiceType = ServiceType.Express,
                DeclaredValueInEuros = 100.0f,
            }
        };

        _context.Parcels.AddRange(parcels);
        await _context.SaveChangesAsync();
    }

    private async Task SeedCasesAsync()
    {
        var regionId = await _context.Regions.Select(r => r.Id).FirstAsync();
        var customer = await _context.Customers.FirstAsync();

        var operatorUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "operator");
        var supervisorUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "supervisor");

        var opHandler = new Handler { UserId = operatorUser!.Id, Department = "Customer Support", MaxCases = 10 };
        var supHandler = new Handler { UserId = supervisorUser!.Id, Department = "Escalations", MaxCases = 5 };
        _context.Handlers.AddRange(opHandler, supHandler);
        await _context.SaveChangesAsync();

        var cases = new List<Case>
        {
            new Case
            {
                CaseNumber = await _caseNumberGenerator.GenerateNextAsync(),
                Title = "Lost Parcel Claim",
                CaseType = CaseType.Lost,
                Status = CaseStatus.Open,
                Priority = Priority.Medium,
                Customer = customer,
                Handler = null,
                RegionId = regionId,
                CreatedDate = DateTime.UtcNow,
                SlaDeadline = DateTime.UtcNow.AddHours(48)
            },

            new Case
            {
                CaseNumber = await _caseNumberGenerator.GenerateNextAsync(),
                Title = "Address Clarification Needed",
                CaseType = CaseType.WrongAddress,
                Status = CaseStatus.AwaitingCustomer,
                Priority = Priority.Low,
                Customer = customer,
                Handler = opHandler,
                RegionId = regionId,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                SlaDeadline = DateTime.UtcNow.AddHours(48)
            },

            new Case
            {
                CaseNumber = await _caseNumberGenerator.GenerateNextAsync(),
                Title = "Billing Adjustment",
                CaseType = CaseType.Billing,
                Status = CaseStatus.Resolved,
                Priority = Priority.Medium,
                Customer = customer,
                Handler = opHandler,
                RegionId = regionId,
                CreatedDate = DateTime.UtcNow.AddDays(-3),
                ResolvedDate = DateTime.UtcNow.AddDays(-1),
                SlaDeadline = DateTime.UtcNow.AddHours(72),
                Resolution = "Refunded 5 EUR to customer account."
            },

            new Case
            {
                CaseNumber = await _caseNumberGenerator.GenerateNextAsync(),
                Title = "Urgent Delivery Change Failed",
                CaseType = CaseType.DeliveryChange,
                Status = CaseStatus.Escalated,
                Priority = Priority.Critical,
                IsEscalated = true,
                Customer = customer,
                Handler = supHandler,
                RegionId = regionId,
                CreatedDate = DateTime.UtcNow.AddHours(-25),
                SlaDeadline = DateTime.UtcNow.AddHours(-1)
            },

            new Case
            {
                CaseNumber = await _caseNumberGenerator.GenerateNextAsync(),
                Title = "General Inquiry",
                CaseType = CaseType.Other,
                Status = CaseStatus.Closed,
                Priority = Priority.Low,
                Customer = customer,
                Handler = opHandler,
                RegionId = regionId,
                CreatedDate = DateTime.UtcNow.AddDays(-10),
                ResolvedDate = DateTime.UtcNow.AddDays(-9),
                SlaDeadline = DateTime.UtcNow.AddHours(72),
                Resolution = "Answered customer question about pricing."
            }
        };

        _context.Cases.AddRange(cases);
        await _context.SaveChangesAsync();
    }
}