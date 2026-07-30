using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Api;

public class MockCaseService : ICaseIntegrationService
{
    public Task<CaseStatusResponse?> GetCaseStatusAsync(string caseNumber, CancellationToken cancellationToken = default)
    {
        if (caseNumber == "999") 
        {
            return Task.FromResult<CaseStatusResponse>(null);
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

    public Task<CreateCaseResponse?> CreateCaseAsync(CreateCaseRequest request, CancellationToken cancellationToken = default)
    {
        var randomCaseNumber = $"CASE-{Random.Shared.Next(10000, 99999)}";
        return Task.FromResult(new CreateCaseResponse(randomCaseNumber));
    }

    public Task AddCaseNoteAsync(string caseNumber, AddCaseNoteRequest request, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}