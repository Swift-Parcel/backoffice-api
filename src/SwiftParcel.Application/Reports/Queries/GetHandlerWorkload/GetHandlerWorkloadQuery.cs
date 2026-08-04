using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Reports.Queries.GetHandlerWorkload;

public record GetHandlerWorkloadQuery : IRequest<Result<IReadOnlyList<HandlerWorkloadReportDto>>>;