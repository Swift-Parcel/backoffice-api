using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Commands.CreateCase;

public record CreateCaseCommand(
    string Title,
    string Description,
    CaseType CaseType,
    CaseStatus CaseStatus,
    Priority Priority,
    string CustomerEmail,
    int RegionId,
    Channel Channel,
    IReadOnlyCollection<int> TagIds,
    IReadOnlyCollection<int> ParcelIds) : IRequest<Result<int>>;