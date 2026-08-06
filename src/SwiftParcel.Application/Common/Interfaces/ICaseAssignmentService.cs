using SwiftParcel.Application.Cases.Dtos;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Common.Interfaces;

public interface ICaseAssignmentService
{
    Task<Result<CaseSummaryDto>> AssignCaseAsync(string caseNumber, int handlerId, CancellationToken cancellationToken = default);
}