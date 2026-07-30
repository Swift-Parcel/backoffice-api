using SwiftParcel.Application.DTO;
using SwiftParcel.Application.Integration.Models;

namespace SwiftParcel.Application.Integration.Interfaces;

public interface ICaseIntegrationService
{
    Task<CaseStatusResponse?> GetCaseStatusAsync(string caseNumber, CancellationToken cancellationToken = default);
    Task<CreateCaseResponse?> CreateCaseAsync(CreateCaseRequest request, CancellationToken cancellationToken = default);
    Task AddCaseNoteAsync(string caseNumber, AddCaseNoteRequest request, CancellationToken cancellationToken = default);
    Task AddCaseFeedbackAsync(string caseNumber, AddCaseFeedbackRequest request, CancellationToken cancellationToken = default);
}