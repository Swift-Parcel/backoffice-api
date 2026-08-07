using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.DeliveryChangeCommand;

public record ProcessDeliveryChangeCommand(
    string CaseNumber, 
    DeliveryChangeOutcome Outcome
) : IRequest<Result<Unit>>;