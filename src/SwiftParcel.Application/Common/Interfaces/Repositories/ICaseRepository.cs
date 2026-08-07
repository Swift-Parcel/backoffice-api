using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Common.Interfaces.Repositories;

using SwiftParcel.Domain.Entities;

public interface ICaseRepository
{
    Task AddAsync(Case newCase, CancellationToken cancellationToken = default);
    Task<Case?> GetByCaseNumberAsync(string caseNumber, CancellationToken cancellationToken = default);
    Task UpdateAsync(Case caseEntity, CancellationToken cancellationToken = default);
    Task<Case?> GetByCaseNumberWithCustomerAsync(string caseNumber, CancellationToken cancellationToken = default);
    Task<List<Tag>> GetTagsByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCaseNumberAsync(string caseNumber, CancellationToken cancellationToken = default);
    Task<List<CaseNoteDto>> GetCaseNotesAsync(string caseNumber, CancellationToken cancellationToken = default);
    Task<CaseStatusResponse?> GetCaseStatusAsync(string caseNumber, CancellationToken cancellationToken = default);
    Task<List<CustomerFacingCaseNoteDto>> GetCustomerCaseNotesAsync(string caseNumber, CancellationToken cancellationToken = default);
    Task<List<CustomerCaseItemDto>> GetCustomerCasesByEmailAsync(string customerEmail, CancellationToken cancellationToken = default);
    Task<List<CaseDto>> GetFilteredCasesAsync(
        IEnumerable<int>? allowedRegionIds,
        bool canAccessAllRegions,
        int? customerId,
        string? customerEmail,
        string? customerPhone,
        CancellationToken cancellationToken = default);
}