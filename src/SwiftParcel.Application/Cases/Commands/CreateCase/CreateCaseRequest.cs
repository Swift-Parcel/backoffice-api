using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Commands.CreateCase;

public record CreateCaseRequest(
    string Title,
    string Description,
    CaseType CaseType,
    CaseStatus CaseStatus,
    string CustomerEmail,
    int? HandlerId,
    Channel Channel,
    IReadOnlyCollection<int> TagIds,
    IReadOnlyCollection<int> ParcelIds,
    int? RegionId = null,
    Priority Priority = Priority.Low);