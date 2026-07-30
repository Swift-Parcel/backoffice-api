using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Infrastructure.Services;

using Application.Integration.Interfaces;
using Persistence;


public class CaseIntegrationService : ICaseIntegrationService
{
    private readonly AppDbContext _dbcontext;

    public CaseIntegrationService(AppDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    public async Task<CaseStatusResponse?> GetCaseStatusAsync(string caseNumber, CancellationToken cancellationToken = default)
    {
        var caseEntity = await _dbcontext.Cases
            .Include(c => c.Notes)
            .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber, cancellationToken);
        
        if(caseEntity == null)
            return null;
        
        var notesDto = caseEntity.Notes.Select(n => new CaseNoteDto(n.CreatedDate, n.NoteText)).ToList();
        
        return new CaseStatusResponse(caseEntity.Status, notesDto, caseEntity.Resolution);
    }

    public async Task<CustomerCasesResponse> GetCustomerCasesAsync(string customerEmail, CancellationToken cancellationToken = default)
    {
        var cases = await _dbcontext.Cases
            .Include(c => c.Customer)
            .Where(c => c.Customer.Email == customerEmail)
            .Select(c => new CustomerCaseItemDto(
                c.CaseNumber,
                c.CaseType,
                c.Status,
                c.CreatedDate,
                c.UpdatedDate
            )).ToListAsync(cancellationToken);

        return new CustomerCasesResponse(cases);
    }

    public Task<CreateCaseResponse?> CreateCaseAsync(CreateCaseRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AddCaseNoteAsync(string caseNumber, AddCaseNoteRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AddCaseFeedbackAsync(string caseNumber, AddCaseFeedbackRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}