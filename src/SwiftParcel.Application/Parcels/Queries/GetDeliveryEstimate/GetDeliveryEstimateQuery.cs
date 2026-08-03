using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;

namespace SwiftParcel.Application.Parcels.Queries.GetDeliveryEstimate;

public record GetDeliveryEstimateQuery(string TrackingNumber) : IRequest<Result<DeliveryEstimateResponse>>;