using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Reports.Queries.GetAverageResolutionTime;

public record GetAverageResolutionTimeQuery : IRequest<Result<IReadOnlyList<AverageResolutionTimeReportDto>>>;