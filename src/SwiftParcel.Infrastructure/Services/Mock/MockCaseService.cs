using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Infrastructure.Services.Mock;

public class MockCaseService : ICaseIntegrationService
{
    public Task<CaseStatusResponse?> GetCaseStatusAsync(string caseNumber, CancellationToken cancellationToken = default)
    {
        if (caseNumber == "999") 
        {
            return Task.FromResult<CaseStatusResponse?>(null);
        }

        var response = new CaseStatusResponse(
            CaseStatus.InProgress,
            new List<CaseNoteDto>
            {
                new(DateTime.UtcNow.AddHours(-3), "Case Started")
            },
            null
        );

        return Task.FromResult<CaseStatusResponse?>(response);
    }

    public Task<CustomerCasesResponse?> GetCustomerCasesAsync(string customerEmail, CancellationToken cancellationToken = default)
    {
        if (customerEmail == "notfound@example.com")
        {
            return Task.FromResult<CustomerCasesResponse?>(null);
        }

        var response = new CustomerCasesResponse(new List<CustomerCaseItemDto>
        {
            new("CASE-1001", CaseType.Damaged, CaseStatus.Open, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1)),
            new("CASE-1002", CaseType.Delayed, CaseStatus.Resolved, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-5))
        });

        return Task.FromResult<CustomerCasesResponse?>(response);
    }

    public Task<CreateCaseResponse?> CreateCaseAsync(CreateCaseRequest request, CancellationToken cancellationToken = default)
    {
        if (request?.CustomerEmail == "error@example.com")
        {
            return Task.FromResult<CreateCaseResponse?>(null);
        }

        var randomCaseNumber = $"CASE-{Random.Shared.Next(10000, 99999)}";
        return Task.FromResult<CreateCaseResponse?>(new CreateCaseResponse(randomCaseNumber));
    }

    public Task AddCaseNoteAsync(string caseNumber, AddCaseNoteRequest request, CancellationToken cancellationToken = default)
    {
        if (caseNumber == "999")
        {
            return Task.CompletedTask; 
        }

        return Task.CompletedTask;
    }

    public Task AddCaseFeedbackAsync(string caseNumber, AddCaseFeedbackRequest request, CancellationToken cancellationToken = default)
    {
        if (caseNumber == "999")
        {
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}