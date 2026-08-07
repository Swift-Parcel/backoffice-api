namespace SwiftParcel.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Entities;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AnyAsync(c => c.Email == email, cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}