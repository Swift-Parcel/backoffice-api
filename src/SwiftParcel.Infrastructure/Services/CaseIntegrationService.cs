using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;

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

    public async Task<CreateCaseResponse?> CreateCaseAsync(CreateCaseRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await  _dbcontext.Customers
            .FirstOrDefaultAsync(c => c.Email == request.CustomerEmail, cancellationToken);
        
        if (customer == null)
            return null;

        var lastcase = await _dbcontext.Cases.OrderByDescending(c => c.CaseNumber).FirstOrDefaultAsync(cancellationToken);
        int nextSequence = 1;
        var currentYear = DateTime.UtcNow.Year.ToString(); // "2026" (vagy amilyen év van)

        if (lastcase is not null && !string.IsNullOrEmpty(lastcase.CaseNumber))
        {
            var parts = lastcase.CaseNumber.Split('-');
    
            if (parts.Length > 2)
            {
                string lastYear = parts[1];
        
                if (int.TryParse(parts[2], out int lastParsedNumber))
                {
                    if (lastYear == currentYear)
                        nextSequence = lastParsedNumber + 1;
                    else
                        nextSequence = 1;
                }
            }
        }

        var caseNumber = $"CASE-{currentYear}-{nextSequence:D3}";
        var now =  DateTime.UtcNow;

        var slaRule = await _dbcontext.SlaRules
            .FirstOrDefaultAsync(r => r.IsActive && r.CaseType == request.CaseType, cancellationToken);

        int slaHours = slaRule?.SlaHours ?? 24;
        var slaDeadline = now.AddHours(slaHours);
        
        var caseEntity = new Case
        {
            CaseNumber = caseNumber,
            CustomerId = customer.Id,
            CaseType = request.CaseType,
            Status = CaseStatus.Open,
            Description = request.Description,
            CreatedDate = now,
            UpdatedDate = now,
            SlaDeadline = slaDeadline,
            Resolution = string.Empty
        };
        
        _dbcontext.Cases.Add(caseEntity);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        return new CreateCaseResponse(caseNumber);

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