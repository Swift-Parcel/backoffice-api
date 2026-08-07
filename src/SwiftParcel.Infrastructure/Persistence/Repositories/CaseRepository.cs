using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Infrastructure.Persistence.Repositories;

public class CaseRepository : ICaseRepository
{
    private readonly AppDbContext _context;

    public CaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Case newCase, CancellationToken cancellationToken = default)
    {
        await _context.Cases.AddAsync(newCase, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Case?> GetByCaseNumberAsync(string caseNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Cases
            .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber, cancellationToken);
    }

    public async Task UpdateAsync(Case caseEntity, CancellationToken cancellationToken = default)
    {
        _context.Cases.Update(caseEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}