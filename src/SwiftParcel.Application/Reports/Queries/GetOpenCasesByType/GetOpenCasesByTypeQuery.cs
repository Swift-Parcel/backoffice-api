using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Reports.Queries.GetOpenCasesByType;

public record GetOpenCasesByTypeQuery : IRequest<Result<IReadOnlyList<CasesByTypeReportDto>>>;