using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Integration.Interfaces;

public interface ICaseService
{
    Task<CaseStatusResponse?> GetCaseStatusAsync(string caseNumber, CancellationToken cancellationToken = default);
    Task<CustomerCasesResponse> GetCustomerCasesAsync(string customerEmail, CancellationToken cancellationToken = default);
    Task<CreateCaseResponse?> CreateCaseAsync(CreateCaseRequest request, CancellationToken cancellationToken = default);
    Task AddCaseFeedbackAsync(string caseNumber, AddCaseFeedbackRequest request, CancellationToken cancellationToken = default);
}