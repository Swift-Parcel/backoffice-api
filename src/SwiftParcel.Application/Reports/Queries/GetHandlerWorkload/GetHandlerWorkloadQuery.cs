using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Reports.Queries.GetHandlerWorkload;

public record GetHandlerWorkloadQuery : IRequest<Result<IReadOnlyList<HandlerWorkloadReportDto>>>;