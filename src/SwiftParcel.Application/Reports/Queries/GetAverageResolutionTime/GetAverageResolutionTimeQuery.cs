using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Reports.Queries.GetAverageResolutionTime;

public record GetAverageResolutionTimeQuery : IRequest<Result<IReadOnlyList<AverageResolutionTimeReportDto>>>;