using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Commands.ProcessDeliveryChange;

public record ProcessDeliveryChangeCommand(
    string CaseNumber, 
    DeliveryChangeOutcome Outcome
) : IRequest<Result<Unit>>;