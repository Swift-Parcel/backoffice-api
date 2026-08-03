using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Parcels.Commands.ConfirmDelivery;

public record ConfirmDeliveryCommand(string TrackingNumber) : IRequest<Result<bool>>;