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
}