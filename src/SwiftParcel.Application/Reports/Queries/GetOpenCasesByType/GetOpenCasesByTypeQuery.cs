using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Reports.Queries.GetOpenCasesByType;

public record GetOpenCasesByTypeQuery : IRequest<Result<IReadOnlyList<CasesByTypeReportDto>>>;