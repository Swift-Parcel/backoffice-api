using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Extensions;

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

    public async Task<Case?> GetByCaseNumberWithCustomerAsync(string caseNumber,
        CancellationToken cancellationToken = default)
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

    public async Task<PagedList<CaseNoteDto>> GetPagedCaseNotesAsync(
        string caseNumber,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.CaseNotes
            .AsNoTracking()
            .Where(n => n.Case.CaseNumber == caseNumber)
            .OrderByDescending(n => n.CreatedDate)
            .Select(n => new CaseNoteDto(
                n.CreatedDate,
                n.NoteText,
                n.HandlerId,
                n.Handler!.User.FullName,
                n.CustomerId,
                n.Customer!.FullName,
                n.Attachment
            ));
        return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<List<CustomerFacingCaseNoteDto>> GetCustomerCaseNotesAsync(string caseNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.CaseNotes
            .AsNoTracking()
            .Where(n => n.Case.CaseNumber == caseNumber && !n.IsInternal)
            .OrderBy(n => n.CreatedDate)
            .Select(n => new CustomerFacingCaseNoteDto(
                n.CreatedDate,
                n.NoteText))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CustomerCaseItemDto>> GetCustomerCasesByEmailAsync(string customerEmail,
        CancellationToken cancellationToken = default)
    {
        return await _context.Cases
            .Include(c => c.Customer)
            .AsNoTracking()
            .Where(c => c.Customer.Email == customerEmail)
            .Select(c => new CustomerCaseItemDto(
                c.CaseNumber,
                c.CaseType,
                c.Status,
                c.CreatedDate,
                c.UpdatedDate
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<CaseStatusResponse?> GetCaseStatusAsync(string caseNumber,
        CancellationToken cancellationToken = default)
    {
        var caseEntity = await _context.Cases
            .Include(c => c.Notes)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber, cancellationToken);

        if (caseEntity == null)
            return null;

        var notesDto = caseEntity.Notes
            .Where(n => !n.IsInternal)
            .OrderBy(n => n.CreatedDate)
            .Select(n => new CustomerFacingCaseNoteDto(
                n.CreatedDate,
                n.NoteText))
            .ToList();

        return new CaseStatusResponse(caseEntity.Status, notesDto, caseEntity.Resolution);
    }

    public async Task<PagedList<Case>> GetCasesFilteredPagedAsync(
        IEnumerable<int>? allowedRegionIds,
        bool canAccessAllRegions,
        int? customerId,
        string? customerEmail,
        string? customerPhone,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Cases
            .Include(c => c.Customer)
            .Include(c => c.Region)
            .Include(c => c.Tags)
            .Include(c => c.Handler.User)
            .AsQueryable();

        if (!canAccessAllRegions)
        {
            var regionIds = allowedRegionIds?.ToList() ?? new List<int>();
            if (!regionIds.Any())
            {
                return new PagedList<Case>(new List<Case>(), 0, 1, 10);
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

        query = query.OrderByDescending(c => c.CreatedDate);

        return await query
            .AsNoTracking()
            .ToPagedListAsync(
                pageNumber,
                pageSize,
                cancellationToken
            );
    }
}