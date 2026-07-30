namespace SwiftParcel.Infrastructure.Services;

using Application.Integration.Interfaces;
using SwiftParcel.Application.Integration.Models;
using Application.DTO;
using Persistence;


public class CaseIntegrationService : ICaseIntegrationService
{
    private readonly AppDbContext _dbcontext;

    public CaseIntegrationService(AppDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    public Task<CaseStatusResponse?> GetCaseStatusAsync(string caseNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CustomerCasesResponse> GetCustomerCasesAsync(string customerEmail, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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