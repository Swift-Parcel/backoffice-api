using MediatR;
using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Reports.Queries;

public record GetSlaBreachesQuery : IRequest<SlaBreachesReportDto>;