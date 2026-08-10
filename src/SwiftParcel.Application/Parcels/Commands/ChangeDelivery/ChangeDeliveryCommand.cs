using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Parcels.Commands.ChangeDelivery;

public record ChangeDeliveryCommand(
    string TrackingNumber,
    DateTime? Date,
    Timeslot? Timeslot
) : IRequest<Result<DeliveryChangeResponse>>
{
};