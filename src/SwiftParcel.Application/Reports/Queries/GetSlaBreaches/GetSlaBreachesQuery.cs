using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Reports.Queries.GetSlaBreaches;

public record GetSlaBreachesQuery : IRequest<Result<SlaBreachesReportDto>>;