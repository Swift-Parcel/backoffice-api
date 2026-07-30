using SwiftParcel.Application.DTO;
using SwiftParcel.Application.Integration.Models;

namespace SwiftParcel.Application.Integration.Interfaces;

public interface ICaseIntegrationService
{
    Task<CaseStatusResponse?> GetCaseStatusAsync(string caseNumber, CancellationToken cancellationToken = default);
}