using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;

namespace SwiftParcel.Application.Parcels.Queries.GetParcelTracking;

public record GetParcelTrackingQuery(string TrackingNumber) : IRequest<Result<ParcelTrackingResponse>>;