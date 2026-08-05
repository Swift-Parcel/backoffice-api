using MediatR;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Parcels.Commands.ChangeDelivery;

public record ChangeDeliveryCommand(
    string TrackingNumber,
    DateTime Date,
    string Timeslot
) : IRequest<Result<DeliveryChangeResponse>>;