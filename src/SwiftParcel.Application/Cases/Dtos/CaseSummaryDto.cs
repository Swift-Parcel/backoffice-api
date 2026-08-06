using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Dtos;

public record CaseSummaryDto(
    int Id,
    string CaseNumber,
    string Title,
    string Description,
    CaseType CaseType,
    CaseStatus Status,
    Priority Priority,
    int CustomerId,
    int? HandlerId,
    DateTime CreatedDate,
    DateTime UpdatedDate,
    bool IsEscalated,
    DateTime? ResolvedDate,
    DateTime SlaDeadline,
    int RegionId,
    Channel Channel,
    string? Resolution,
    int? SatisfactionScore,
    
    IReadOnlyCollection<TagDto> Tags,
    
    int NoteCount,
    int ParcelCount);