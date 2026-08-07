using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.DTO.Cases;
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
    
    public async Task<Case?> GetByCaseNumberWithCustomerAsync(string caseNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Cases
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber, cancellationToken);
    }
    
    public async Task<List<Tag>> GetTagsByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .Where(t => ids.Contains(t.Id))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<bool> ExistsByCaseNumberAsync(string caseNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Cases
            .AnyAsync(c => c.CaseNumber == caseNumber, cancellationToken);
    }

    public async Task<List<CaseNoteDto>> GetCaseNotesAsync(string caseNumber, CancellationToken cancellationToken = default)
    {
        return await _context.CaseNotes
            .AsNoTracking()
            .Where(n => n.Case.CaseNumber == caseNumber)
            .OrderBy(n => n.CreatedDate)
            .Select(n => new CaseNoteDto(
                n.CreatedDate,
                n.NoteText, 
                n.HandlerId, 
                n.Handler!.FullName, 
                n.CustomerId, 
                n.Customer!.FullName,  
                n.Attachment))
            .ToListAsync(cancellationToken);
    }
    public async Task<List<CaseDto>> GetFilteredCasesAsync(
    IEnumerable<int>? allowedRegionIds,
    bool canAccessAllRegions,
    int? customerId,
    string? customerEmail,
    string? customerPhone,
    CancellationToken cancellationToken = default)
{
    var query = _context.Cases.AsNoTracking();

    if (!canAccessAllRegions)
    {
        var regionIds = allowedRegionIds?.ToList() ?? new List<int>();
        if (!regionIds.Any())
        {
            return new List<CaseDto>();
        }

        query = query.Where(c => regionIds.Contains(c.RegionId));
    }

    if (customerId.HasValue)
    {
        query = query.Where(c => c.CustomerId == customerId.Value);
    }

    if (!string.IsNullOrWhiteSpace(customerEmail))
    {
        query = query.Where(c => c.Customer.Email.ToLower() == customerEmail.ToLower());
    }

    if (!string.IsNullOrWhiteSpace(customerPhone))
    {
        query = query.Where(c => c.Customer.Phone == customerPhone);
    }

    return await query
        .Select(c => new CaseDto
        {
            Id = c.Id,
            CaseNumber = c.CaseNumber,
            Title = c.Title,
            Description = c.Description,
            CaseType = c.CaseType,
            Status = c.Status,
            Priority = c.Priority,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate,
            IsEscalated = c.IsEscalated,
            ResolvedDate = c.ResolvedDate,
            SlaDeadline = c.SlaDeadline,
            Channel = c.Channel,
            Resolution = c.Resolution,
            SatisfactionScore = c.SatisfactionScore,

            CustomerId = c.CustomerId,
            CustomerName = c.Customer.FullName,

            RegionId = c.RegionId,
            RegionName = c.Region.Name,

            HandlerId = c.HandlerId,
            HandlerName = c.Handler != null ? c.Handler.User.FullName : null,

            Tags = c.Tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList()
        })
        .ToListAsync(cancellationToken);
}
}